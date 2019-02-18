using SFA.DAS.EmployerFinance.Models.ExpiringFunds;

namespace SFA.DAS.EmployerFinance.Services
{
    public interface IForecastingService
    {
        ExpiringAccountFunds GetExpiringAccountFunds(long accountId);
    }
}
