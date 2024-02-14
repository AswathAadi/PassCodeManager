using PassCodeManager.DTO.RequestObjects;
using PassCodeManager.DTO.ResponseObjects;

namespace PassCodeManager.Services.Abstract
{
    public interface ISecurityService
    {
        Task<SecurityResponseObject> AddPasscode(AddPasscodeObject passcodeObject);
        Task<Dictionary<string, string>> GetPassCodesByMobile(string mobile);
    }
}
