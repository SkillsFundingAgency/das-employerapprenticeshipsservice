using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.Levy;

namespace SFA.DAS.EmployerAccounts.Data
{
    public interface IDasLevyRepository
    {
        Task<IEnumerable<DasEnglishFraction>> GetEnglishFractionHistory(long accountId, string empRef);
        Task<List<AccountBalance>> GetAccountBalances(List<long> accountIds);
    }
}
