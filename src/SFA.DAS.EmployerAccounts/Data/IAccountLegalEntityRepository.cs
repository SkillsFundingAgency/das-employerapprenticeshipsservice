using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Data;

public interface IAccountLegalEntityRepository
{
    Task<List<AccountLegalEntity>> GetAccountLegalEntities(string accountHashedId);
}