using Microsoft.EntityFrameworkCore;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.PAYE;

namespace SFA.DAS.EmployerAccounts.Data.Contracts;

public interface IEmployerAccountsDbContext
{
    DbSet<AccountLegalEntity> AccountLegalEntities { get; set; }
    DbSet<Account> Accounts { get; set; }
    DbSet<AccountHistory> AccountHistory { get; set; }
    DbSet<EmployerAgreement> Agreements { get; set; }
    DbSet<AgreementTemplate> AgreementTemplates { get; set; }
    DbSet<HealthCheck> HealthChecks { get; set; }
    DbSet<LegalEntity> LegalEntities { get; set; }
    DbSet<Membership> Memberships { get; set; }
    DbSet<User> Users { get; set; }
    DbSet<UserAccountSetting> UserAccountSettings { get; set; }
    DbSet<RunOnceJob> RunOnceJobs { get; set; }
    DbSet<Paye> Payees { get; set; }
}