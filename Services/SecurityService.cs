using Microsoft.EntityFrameworkCore;
using PassCodeManager.Classified.DBcontext;
using PassCodeManager.Classified.Entities;
using PassCodeManager.DTO.RequestObjects;
using PassCodeManager.DTO.ResponseObjects;
using PassCodeManager.Services.Abstract;
using System.Security.Cryptography;
using System.Text;

namespace PassCodeManager.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly Context _context;

        public SecurityService(Context context)
        {
            _context = context;
        }

        public async Task<SecurityResponseObject> AddPasscode(AddPasscodeObject passcodeObject)
        {
            try
            {
                await _context.Database.BeginTransactionAsync();

                var securityKeyEntity = new TblSecurityKeys()
                {
                    Id = Guid.NewGuid().ToString(),
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now,
                    IsActive = true
                };

                var passCodeEntity = new TblPassCodes()
                {
                    CreatedOn = DateTime.Now,
                    Id = Guid.NewGuid().ToString(),
                    IsActive = true,
                    Mobile = passcodeObject.Mobile,
                    Name = passcodeObject.Name,
                };

                using (RSA rsa = RSA.Create())
                {
                    string publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey()); // Public Key Generation

                    byte[] originalBytes = Encoding.UTF8.GetBytes(passcodeObject.PassCode);
                    byte[] encryptedBytes = rsa.Encrypt(originalBytes, RSAEncryptionPadding.Pkcs1);
                    string encryptedString = Convert.ToBase64String(encryptedBytes);

                    passCodeEntity.PassCode = encryptedString;

                    string privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey()); // Private Key Generation

                    securityKeyEntity.PublicKey = publicKey;
                    securityKeyEntity.PrivateKey = privateKey;
                    securityKeyEntity.Mobile = passcodeObject.Mobile;
                    securityKeyEntity.PassCodeId = passCodeEntity.Id;
                }

                securityKeyEntity.HasPassCodes.Add(passCodeEntity);
                await _context.AddAsync(securityKeyEntity);

                await _context.Database.CommitTransactionAsync();

                await _context.SaveChangesAsync();

                return new SecurityResponseObject();
            }
            catch (Exception ex)
            {
                await _context.Database.RollbackTransactionAsync();
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Dictionary<string, string>> GetPassCodesByMobile(string mobile)
        {
            try
            {
                Dictionary<string, string> passCodes = new Dictionary<string, string>();
                var EncryptedPasscode = await _context.SecurityKeys.Include(x => x.HasPassCodes).FirstOrDefaultAsync(x => x.Mobile == mobile);

                foreach (var EncryptedString in EncryptedPasscode.HasPassCodes)
                {
                    byte[] encryptedBytes = Convert.FromBase64String(EncryptedString.PassCode);

                    byte[] decryptedBytes;
                    using (RSA rsa = RSA.Create())
                    {
                        rsa.ImportRSAPrivateKey(Convert.FromBase64String(EncryptedPasscode.PrivateKey), out _);
                        decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.Pkcs1);
                    }

                    string decryptedString = Encoding.UTF8.GetString(decryptedBytes);
                    passCodes.Add(EncryptedString.Name, decryptedString);
                }
                
                return passCodes;
            }
            catch (Exception ex)
            {
                await _context.DisposeAsync();
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
