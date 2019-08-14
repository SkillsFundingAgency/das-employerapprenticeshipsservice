using SFA.DAS.EAS.Domain.Models.Account;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface IEmployerAccountRepository
    {
        Task<Account> GetAccountById(long id);
        Task<Account> GetAccountByHashedId(string hashedAccountId);
        Task<AccountDetail> GetAccountDetailByHashedId(string hashedAccountId);
        Task<List<Account>> GetAllAccounts();
        Task<List<AccountHistoryEntry>> GetAccountHistory(long accountId);
    }
}
