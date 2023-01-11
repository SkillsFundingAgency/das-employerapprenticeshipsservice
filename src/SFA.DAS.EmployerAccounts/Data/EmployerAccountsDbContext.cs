using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.PAYE;

namespace SFA.DAS.EmployerAccounts.Data;

public class EmployerAccountsDbContext : DbContext
{
    private readonly EmployerAccountsConfiguration _configuration;
    private readonly AzureServiceTokenProvider _azureServiceTokenProvider;
 
    private const string AzureResource = "https://database.windows.net/";

    public DbSet<AccountLegalEntity> AccountLegalEntities { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<AccountHistory> AccountHistory { get; set; }
    public DbSet<EmployerAgreement> Agreements { get; set; }
    public DbSet<AgreementTemplate> AgreementTemplates { get; set; }
    public DbSet<HealthCheck> HealthChecks { get; set; }
    public DbSet<LegalEntity> LegalEntities { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserAccountSetting> UserAccountSettings { get; set; }
    public DbSet<RunOnceJob> RunOnceJobs { get; set; }
    public DbSet<Paye> Payees { get; set; }

    public EmployerAccountsDbContext(DbContextOptions options): base(options) { }

    public EmployerAccountsDbContext(IOptions<EmployerAccountsConfiguration> configuration, DbContextOptions options, AzureServiceTokenProvider azureServiceTokenProvider) : base(options)
    {
        _configuration = configuration.Value;
        _azureServiceTokenProvider = azureServiceTokenProvider;
    }


    //public EmployerAccountsDbContext(DbConnection connection, DbTransaction transaction = null)
    //    : base(connection, false)
    //{
    //    if (transaction == null) Database.BeginTransaction();
    //    else Database.UseTransaction(transaction);
    //}


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();

        if (_configuration == null || _azureServiceTokenProvider == null)
        {
            optionsBuilder.UseSqlServer().UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            return;
        }

        var connection = new SqlConnection
        {
            ConnectionString = _configuration.DatabaseConnectionString,
            AccessToken = _azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result,
        };

        optionsBuilder.UseSqlServer(connection, options =>
            options.EnableRetryOnFailure(
                5,
                TimeSpan.FromSeconds(20),
                null
            )).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("employer_account");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EmployerAccountsDbContext).Assembly);
    }
}