using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Data;

public class AccountLegalEntityRepository : BaseRepository, IAccountLegalEntityRepository
{
    private readonly Lazy<EmployerAccountsDbContext> _db;

    public AccountLegalEntityRepository(EmployerAccountsConfiguration configuration, ILogger<AccountLegalEntityRepository> logger, Lazy<EmployerAccountsDbContext> db)
        : base(configuration.DatabaseConnectionString, logger)
    {
        _db = db;
    }

    public async Task<List<AccountLegalEntity>> GetAccountLegalEntities(string accountHashedId)
    {
        var accountLegalEntities = await _db.Value.AccountLegalEntities.Where(l =>
                 l.Account.HashedId == accountHashedId &&
                 (l.PendingAgreementId != null || l.SignedAgreementId != null) &&
                 l.Deleted == null).ToListAsync();

        return accountLegalEntities;
    }
}