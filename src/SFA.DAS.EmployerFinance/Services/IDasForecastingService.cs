using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.ExpiringFunds;

namespace SFA.DAS.EmployerFinance.Services
{
    public interface IDasForecastingService
    {
        Task<ExpiringAccountFunds> GetExpiringAccountFunds(long accountId);
    }
}
