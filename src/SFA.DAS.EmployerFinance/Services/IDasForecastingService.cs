using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.ExpiringFunds;
using SFA.DAS.EmployerFinance.Models.ProjectedCalculations;

namespace SFA.DAS.EmployerFinance.Services
{
    public interface IDasForecastingService
    {
        Task<ExpiringAccountFunds> GetExpiringAccountFunds(long accountId);
        Task<ProjectedCalculation> GetProjectedCalculations(long accountId);
    }
}
