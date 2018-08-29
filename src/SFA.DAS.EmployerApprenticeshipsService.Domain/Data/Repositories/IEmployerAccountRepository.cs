using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface IEmployerAccountRepository
    {
        Task<Models.Account.Account> GetAccountById(long id);
        Task<Models.Account.Account> GetAccountByHashedId(string hashedAccountId);
        Task<Accounts<Models.Account.Account>> GetAccounts(string toDate, int pageNumber, int pageSize);
        Task<AccountDetail> GetAccountDetailByHashedId(string hashedAccountId);
        Task<List<Models.Account.Account>> GetAllAccounts();
        Task<List<AccountHistoryEntry>> GetAccountHistory(long accountId);
        Task RenameAccount(long id, string name);
    }
}
