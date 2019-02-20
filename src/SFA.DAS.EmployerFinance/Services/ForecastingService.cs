using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.ExpiringFunds;

namespace SFA.DAS.EmployerFinance.Services
{
    public class ForecastingService : IForecastingService
    {
        public Task<ExpiringAccountFunds> GetExpiringAccountFunds(long accountId)
        {
            //Placeholder code to allow UI work to continue
            //TODO: Replace this with working API code
            return Task.Run(() => new ExpiringAccountFunds
            {
                AccountId = accountId,
                ProjectionGenerationDate = DateTime.Now,
                ExpiryAmounts = new List<ExpiringFunds>
                {
                    new ExpiringFunds {Amount = 100, PayrollDate = DateTime.Now.AddMonths(2)}
                }
            });
        }
    }
}
