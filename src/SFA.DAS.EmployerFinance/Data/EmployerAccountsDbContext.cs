using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.EmployerFinance.Models.AccountTeam;
using SFA.DAS.EntityFramework;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerFinance.Data
{
    [DbConfigurationType(typeof(SqlAzureDbConfiguration))]
    public class EmployerAccountsDbContext : DbContext
    {
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<EmployerAgreement> Agreements { get; set; }
        public virtual DbSet<Membership> Memberships { get; set; }
        public virtual DbSet<User> Users { get; set; }

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
            modelBuilder.Entity<AgreementTemplate>().ToTable("EmployerAgreementTemplate").HasMany(t => t.Agreements);
            modelBuilder.Entity<EmployerAgreement>().HasRequired(a => a.Account);
            modelBuilder.Entity<EmployerAgreement>().HasRequired(a => a.LegalEntity);
            modelBuilder.Entity<EmployerAgreement>().HasRequired(a => a.Template);
            modelBuilder.Entity<LegalEntity>().HasMany(l => l.Agreements);
            modelBuilder.Entity<Membership>().HasKey(m => new { m.AccountId, m.UserId }).Ignore(m => m.RoleId).Property(m => m.Role).HasColumnName(nameof(Membership.RoleId));
            modelBuilder.Entity<User>().Ignore(u => u.UserRef).Property(u => u.Ref).HasColumnName(nameof(User.UserRef));
        }
    }
}