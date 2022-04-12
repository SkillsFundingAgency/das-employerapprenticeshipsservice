using SFA.DAS.EmployerFinance.Interfaces.OuterApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.Transfers
{
    public class GetFinancialBreakdownRequest : IGetApiRequest
    {
        private readonly long _accountId;

        public GetFinancialBreakdownRequest(long accountId)
        {
            _accountId = accountId;
        }

        public string GetUrl => $"Transfers/{_accountId}/transfers/financial-breakdown";
    }
}
