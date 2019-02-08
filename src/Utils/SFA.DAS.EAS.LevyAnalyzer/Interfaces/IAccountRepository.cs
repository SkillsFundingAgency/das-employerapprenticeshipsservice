using System.Threading.Tasks;
using SFA.DAS.EAS.LevyAnalyser.Models;

namespace SFA.DAS.EAS.LevyAnalyser.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account> GetAccountAsync(long accountId);

        Task<long[]> GetAllAccountIds();
    }
}