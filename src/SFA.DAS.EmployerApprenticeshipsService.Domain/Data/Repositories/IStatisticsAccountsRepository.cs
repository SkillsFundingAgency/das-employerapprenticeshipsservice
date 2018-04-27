using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Data.Entities.Statistics;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface IStatisticsAccountsRepository
    {
        Task<StatisticsAccounts> GetStatistics();
    }
}
