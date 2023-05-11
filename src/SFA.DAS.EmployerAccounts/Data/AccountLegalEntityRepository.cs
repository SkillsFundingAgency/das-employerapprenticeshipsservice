using Microsoft.EntityFrameworkCore;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Data;

public class AccountLegalEntityRepository :  IAccountLegalEntityRepository
{
    private readonly Lazy<EmployerAccountsDbContext> _db;

    public AccountLegalEntityRepository(Lazy<EmployerAccountsDbContext> db)
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