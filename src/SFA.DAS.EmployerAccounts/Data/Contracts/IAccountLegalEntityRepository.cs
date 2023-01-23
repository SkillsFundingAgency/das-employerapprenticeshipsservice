using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Data.Contracts;

public interface IAccountLegalEntityRepository
{
    Task<List<AccountLegalEntity>> GetAccountLegalEntities(string accountHashedId);
}