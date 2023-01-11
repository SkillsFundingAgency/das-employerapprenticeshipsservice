using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.PAYE;

namespace SFA.DAS.EmployerAccounts.Data;

[DbConfigurationType(typeof(SqlAzureDbConfiguration))]
public class EmployerAccountsDbContext : DbContext
{
    public virtual DbSet<AccountLegalEntity> AccountLegalEntities { get; set; }
    public virtual DbSet<Account> Accounts { get; set; }
    public virtual DbSet<AccountHistory> AccountHistory { get; set; }
    public virtual DbSet<EmployerAgreement> Agreements { get; set; }
    public virtual DbSet<AgreementTemplate> AgreementTemplates { get; set; }
    public virtual DbSet<HealthCheck> HealthChecks { get; set; }
    public virtual DbSet<LegalEntity> LegalEntities { get; set; }
    public virtual DbSet<Membership> Memberships { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserAccountSetting> UserAccountSettings { get; set; }
    public virtual DbSet<RunOnceJob> RunOnceJobs { get; set; }
    public virtual DbSet<Paye> Payees { get; set; }

    static EmployerAccountsDbContext()
    {
        Database.SetInitializer<EmployerAccountsDbContext>(null);
    }

    public EmployerAccountsDbContext(string nameOrConnectionString)
        : base(nameOrConnectionString)
    {
    }

    public EmployerAccountsDbContext(DbConnection connection, DbTransaction transaction = null)
        : base(connection, false)
    {
        if (transaction == null) Database.BeginTransaction();
        else Database.UseTransaction(transaction);
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
        modelBuilder.Entity<Account>().Ignore(a => a.role).Ignore(a => a.RoleName);
        modelBuilder.Entity<Account>().HasMany(a => a.AccountLegalEntities);
        modelBuilder.Entity<Account>().HasMany(a => a.Memberships);
        modelBuilder.Entity<Account>().HasMany(a => a.AccountHistory);
        modelBuilder.Entity<AccountLegalEntity>().HasMany(ale => ale.Agreements);
        modelBuilder.Entity<AccountLegalEntity>().HasRequired(ale => ale.Account);
        modelBuilder.Entity<AccountLegalEntity>().HasRequired(ale => ale.LegalEntity);
        modelBuilder.Entity<AccountLegalEntity>().HasOptional(ale => ale.SignedAgreement).WithMany().HasForeignKey(ale => ale.SignedAgreementId);
        modelBuilder.Entity<AccountLegalEntity>().HasOptional(ale => ale.PendingAgreement).WithMany().HasForeignKey(ale => ale.PendingAgreementId);
        modelBuilder.Entity<AgreementTemplate>().ToTable("EmployerAgreementTemplate").HasMany(t => t.Agreements);
        modelBuilder.Entity<EmployerAgreement>().HasRequired(a => a.AccountLegalEntity);
        modelBuilder.Entity<EmployerAgreement>().HasRequired(a => a.Template);
        modelBuilder.Entity<EmployerAgreement>().Ignore(c => c.SignedByEmail);
        modelBuilder.Entity<HealthCheck>().ToTable("HealthChecks", "dbo");
        modelBuilder.Entity<Membership>().HasKey(m => new { m.AccountId, m.UserId });
        modelBuilder.Entity<Paye>().Ignore(a => a.AccountId);
        modelBuilder.Entity<User>().Ignore(u => u.FullName).Ignore(u => u.UserRef).Property(u => u.Ref).HasColumnName(nameof(User.UserRef));
        modelBuilder.Entity<UserAccountSetting>().HasRequired(u => u.Account);
        modelBuilder.Entity<UserAccountSetting>().HasRequired(u => u.User);
        modelBuilder.Entity<UserAccountSetting>().ToTable("UserAccountSettings");
        modelBuilder.Entity<RunOnceJob>().ToTable("RunOnceJob", "dbo");
        modelBuilder.Entity<RunOnceJob>().HasKey(j => j.Name);
    }
}