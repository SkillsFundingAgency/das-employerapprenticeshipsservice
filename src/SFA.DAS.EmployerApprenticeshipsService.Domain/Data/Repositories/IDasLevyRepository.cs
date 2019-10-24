using SFA.DAS.EAS.Domain.Models.Levy;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface IDasLevyRepository
    {
        Task<List<LevyDeclarationView>> GetAccountLevyDeclarations(long accountId);
        Task<List<LevyDeclarationView>> GetAccountLevyDeclarations(long accountId, string payrollYear, short payrollMonth);
        Task<List<AccountBalance>> GetAccountBalances(List<long> accountIds);
    }
}
