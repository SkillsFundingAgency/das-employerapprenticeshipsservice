using SFA.DAS.EmployerFinance.Models.TransferConnections;
using SFA.DAS.EmployerFinance.Models.UserProfile;
using SFA.DAS.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.EmployerFinance.Models.AccountTeam;
using SFA.DAS.EmployerFinance.Models.Paye;

namespace SFA.DAS.EmployerFinance.Data
{
    [DbConfigurationType(typeof(SqlAzureDbConfiguration))]
    public class EmployerAccountsDbContext : DbContext
    {
        public virtual DbSet<AccountLegalEntity> AccountLegalEntities { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<EmployerAgreement> Agreements { get; set; }
        public virtual DbSet<AgreementTemplate> AgreementTemplates { get; set; }
        public virtual DbSet<LegalEntity> LegalEntities { get; set; }
        public virtual DbSet<Membership> Memberships { get; set; }
		public virtual DbSet<TransferConnectionInvitation> TransferConnectionInvitations { get; set; }
        public virtual DbSet<User> Users { get; set; }

        static EmployerAccountsDbContext()
        {
            Database.SetInitializer<EmployerAccountsDbContext>(null);
        }

        public EmployerAccountsDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        protected EmployerAccountsDbContext()
        {
        }

        public virtual Task<List<T>> SqlQueryAsync<T>(string query, params object[] parameters)
        {
            return Database.SqlQuery<T>(query, parameters).ToListAsync();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.HasDefaultSchema("employer_account");
            modelBuilder.Entity<Account>().HasMany(a => a.AccountLegalEntities);
            modelBuilder.Entity<Account>().HasMany(a => a.ReceivedTransferConnectionInvitations).WithRequired(i => i.ReceiverAccount);
            modelBuilder.Entity<Account>().HasMany(a => a.SentTransferConnectionInvitations).WithRequired(i => i.SenderAccount);
            modelBuilder.Entity<AccountLegalEntity>().HasMany(ale => ale.Agreements);
            modelBuilder.Entity<AccountLegalEntity>().HasRequired(ale => ale.Account);
            modelBuilder.Entity<AccountLegalEntity>().HasRequired(ale => ale.LegalEntity);
            modelBuilder.Entity<AccountLegalEntity>().HasOptional(ale => ale.SignedAgreement).WithMany().HasForeignKey(ale => ale.SignedAgreementId);
            modelBuilder.Entity<AccountLegalEntity>().HasOptional(ale => ale.PendingAgreement).WithMany().HasForeignKey(ale => ale.PendingAgreementId);
            modelBuilder.Entity<AgreementTemplate>().ToTable("EmployerAgreementTemplate").HasMany(t => t.Agreements);
            modelBuilder.Entity<EmployerAgreement>().HasRequired(a => a.AccountLegalEntity);
            modelBuilder.Entity<EmployerAgreement>().HasRequired(a => a.Template);
            modelBuilder.Entity<Membership>().HasKey(m => new { m.AccountId, m.UserId }).Ignore(m => m.RoleId).Property(m => m.Role).HasColumnName(nameof(Membership.RoleId));
            modelBuilder.Entity<Paye>().Ignore(a => a.AccountId);
            modelBuilder.Entity<TransferConnectionInvitation>().HasRequired(i => i.ReceiverAccount);
            modelBuilder.Entity<TransferConnectionInvitation>().HasRequired(i => i.SenderAccount);
            modelBuilder.Entity<User>().Ignore(u => u.FullName).Ignore(u => u.UserRef).Property(u => u.Ref).HasColumnName(nameof(User.UserRef));
        }
    }
}