using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.TransferConnections;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerAccounts.Data
{
    [DbConfigurationType(typeof(SqlAzureDbConfiguration))]
    public class EmployerAccountsDbContext : DbContext
    {
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<EmployerAgreement> Agreements { get; set; }
        public virtual DbSet<AgreementTemplate> AgreementTemplates { get; set; }
        public virtual DbSet<LegalEntity> LegalEntities { get; set; }
        public virtual DbSet<Membership> Memberships { get; set; }
        public virtual DbSet<TransferConnectionInvitation> TransferConnectionInvitations { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserAccountSetting> UserAccountSettings { get; set; }

        static EmployerAccountsDbContext()
        {
            Database.SetInitializer<EmployerAccountsDbContext>(null);
        }

        public EmployerAccountsDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public EmployerAccountsDbContext(IUnitOfWorkContext unitOfWorkContext)
            : base(unitOfWorkContext.Get<DbConnection>(), false)
        {
            Database.UseTransaction(unitOfWorkContext.Get<DbTransaction>());
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
            modelBuilder.Entity<Account>().Ignore(a => a.RoleId).Ignore(a => a.RoleName);
            modelBuilder.Entity<Account>().HasMany(a => a.Agreements);
            modelBuilder.Entity<Account>().HasMany(a => a.ReceivedTransferConnectionInvitations).WithRequired(i => i.ReceiverAccount);
            modelBuilder.Entity<Account>().HasMany(a => a.SentTransferConnectionInvitations).WithRequired(i => i.SenderAccount);
            modelBuilder.Entity<AgreementTemplate>().ToTable("EmployerAgreementTemplate").HasMany(t => t.Agreements);
            modelBuilder.Entity<EmployerAgreement>().HasRequired(a => a.Account);
            modelBuilder.Entity<EmployerAgreement>().HasRequired(a => a.LegalEntity);
            modelBuilder.Entity<EmployerAgreement>().HasRequired(a => a.Template);
            modelBuilder.Entity<LegalEntity>().HasMany(l => l.Agreements);
            modelBuilder.Entity<Membership>().HasKey(m => new { m.AccountId, m.UserId }).Ignore(m => m.RoleId).Property(m => m.Role).HasColumnName(nameof(Membership.RoleId));
            modelBuilder.Entity<User>().Ignore(u => u.FullName).Ignore(u => u.UserRef).Property(u => u.Ref).HasColumnName(nameof(User.UserRef));
            modelBuilder.Entity<TransferConnectionInvitation>().HasRequired(i => i.ReceiverAccount);
            modelBuilder.Entity<TransferConnectionInvitation>().HasRequired(i => i.SenderAccount);
            modelBuilder.Entity<UserAccountSetting>().HasRequired(u => u.Account);
            modelBuilder.Entity<UserAccountSetting>().HasRequired(u => u.User);
            modelBuilder.Entity<UserAccountSetting>().ToTable("UserAccountSettings");
        }
    }
}
