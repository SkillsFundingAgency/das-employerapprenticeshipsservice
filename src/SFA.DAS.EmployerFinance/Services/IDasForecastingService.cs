using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.ProjectedCalculations;

namespace SFA.DAS.EmployerFinance.Services
{
    public interface IDasForecastingService
    {
        Task<AccountProjectionSummary> GetAccountProjectionSummary(long accountId);
    }
}
