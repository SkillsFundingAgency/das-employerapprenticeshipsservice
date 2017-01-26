using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Account;
using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Domain.Data
{
    public interface IEmployerAccountRepository
    {
        Task<Account> GetAccountById(long id);
        Task<Account> GetAccountByHashedId(string hashedAccountId);
        Task<Accounts<Account>> GetAccounts(string toDate, int pageNumber, int pageSize);
        Task<Accounts<AccountInformation>> GetAccountsByDateRange(DateTime fromDate, DateTime toDate, int pageNumber, int pageSize);
        Task<List<Account>> GetAllAccounts();
        Task<List<AccountHistoryEntry>> GetAccountHistory(long accountId);
        Task RenameAccount(long id, string name);
    }
}
