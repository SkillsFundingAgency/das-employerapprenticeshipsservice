using SFA.DAS.EAS.LevyAnalyzer.Models;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.LevyAnalyzer.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account> GetAccountAsync(long accountId);
    }
}