using SFA.DAS.EmployerFinance.Models.Account;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface IEmployerAccountRepository
    {
        Task<Account> Get(long id);
        Task<List<Account>> Get(List<long> accountIds);
        Task<Account> Get(string publicHashedId);
        Task<List<Account>> GetAll();
    }
}
