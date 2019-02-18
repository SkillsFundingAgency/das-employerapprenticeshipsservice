using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.ExpiringFunds;

namespace SFA.DAS.EmployerFinance.Services
{
    public class ForecastingService : IForecastingService
    {
        public Task<ExpiringAccountFunds> GetExpiringAccountFunds(long accountId)
        {
            throw new NotImplementedException();
        }
    }
}
