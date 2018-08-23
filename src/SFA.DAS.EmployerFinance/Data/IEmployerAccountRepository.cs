using SFA.DAS.EmployerFinance.Models.Account;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface IEmployerAccountRepository
    {
        Task<List<Account>> GetAllAccounts();
        Task<Account> GetAccountByHashedId(string hashedAccountId);
    }
}
