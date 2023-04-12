using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Data.Contracts;

public interface IEmployerAccountRepository
{
    Task<Account> GetAccountById(long accountId);
    Task<Accounts<Account>> GetAccounts(string toDate, int pageNumber, int pageSize);
    Task<AccountDetail> GetAccountDetailByHashedId(string hashedAccountId);
    Task<AccountStats> GetAccountStats(long accountId);
    Task RenameAccount(long id, string name);
    Task SetAccountLevyStatus(long accountId, ApprenticeshipEmployerType apprenticeshipEmployerType);
}