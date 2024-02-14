using Microsoft.EntityFrameworkCore;
using PassCodeManager.Classified.Entities;

namespace PassCodeManager.Classified.DBcontext
{
    public class Context : DbContext
    {
        #region Db Connection Check.
        public Context(DbContextOptions<Context> options)
        : base(options)
        {
            try
            {
                // Attempt to open the database connection
                bool isConnectionAvailable = Database.CanConnect();
                if (!isConnectionAvailable) { return; }
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                Console.WriteLine($"Error connecting to the database: {ex.Message}");
                throw;
            }
        }
        #endregion

        public DbSet<TblPassCodes> PassCodes { get; set; }

        public DbSet<TblSecurityKeys> SecurityKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblPassCodes>(entity =>
            {
                entity.ToTable("tbl_passcodes");

                entity.HasIndex(e => e.Id)
                .HasDatabaseName("UQ_tbl_passcodes");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(55)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.IsActive)
                    .HasColumnName("is_active")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.PassCode)
                    .HasColumnName("passcode")
                    .HasMaxLength(1024)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedOn).IsRequired(true).HasColumnName("created_on");
                entity.Property(e => e.UpdatedOn).HasColumnName("updated_on");

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(25);
            });

            modelBuilder.Entity<TblSecurityKeys>(entity =>
            {
                entity.ToTable("tbl_security_keys");

                entity.HasIndex(e => e.Id)
                .HasDatabaseName("UQ_tbl_passcodes");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(55)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.PublicKey)
                    .HasColumnName("public_key")
                    .HasMaxLength(1024);

                entity.Property(e => e.IsActive)
                    .HasColumnName("is_active")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.PrivateKey)
                    .HasColumnName("private_key")
                    .HasMaxLength(2048);

                entity.Property(e => e.CreatedOn).IsRequired(true).HasColumnName("created_on");
                entity.Property(e => e.UpdatedOn).HasColumnName("updated_on");

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(25);

                entity.Property(e => e.PassCodeId)
                    .HasColumnName("passcode_id")
                    .HasMaxLength(55)
                    .IsRequired(true);

                entity.HasMany(x => x.HasPassCodes)
                    .WithOne(y => y.SecurityKeys)
                    .HasPrincipalKey(x => x.PassCodeId)
                    .HasForeignKey(x => x.Id);
            });

        }
    }
}
