using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.Account;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface IEmployerAccountRepository
    {
        Task<List<Account>> GetAllAccounts();
    }
}
