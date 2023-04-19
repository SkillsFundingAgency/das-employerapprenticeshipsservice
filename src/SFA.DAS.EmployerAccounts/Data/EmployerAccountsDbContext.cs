using System.Data;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.PAYE;

namespace SFA.DAS.EmployerAccounts.Data;

public class EmployerAccountsDbContext : DbContext, IEmployerAccountsDbContext
{
    private readonly EmployerAccountsConfiguration _configuration;
    private readonly AzureServiceTokenProvider _azureServiceTokenProvider;

    private readonly IDbConnection _connection;

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

    // For tests
    public EmployerAccountsDbContext() { }

    public EmployerAccountsDbContext(DbContextOptions options) : base(options) { }

    public EmployerAccountsDbContext(IDbConnection connection, EmployerAccountsConfiguration configuration, DbContextOptions options, AzureServiceTokenProvider azureServiceTokenProvider) : base(options)
    {
        _configuration = configuration;
        _azureServiceTokenProvider = azureServiceTokenProvider;
        _connection = connection;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();

        if (_configuration == null || _azureServiceTokenProvider == null)
        {
            return;
        }

        optionsBuilder.UseSqlServer(_connection as SqlConnection);

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("employer_account");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EmployerAccountsDbContext).Assembly);
    }
}