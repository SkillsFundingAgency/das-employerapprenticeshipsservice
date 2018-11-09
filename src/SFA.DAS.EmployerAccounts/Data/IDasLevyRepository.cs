using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Data
{
    public interface IDasLevyRepository
    {
        Task<List<AccountBalance>> GetAccountBalances(List<long> accountIds);
    }
}
