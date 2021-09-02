using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests
{
    public class GetCohortsRequest : IGetApiRequest
    {
        private readonly long _accountId;

        public GetCohortsRequest(long accountId)
        {
            _accountId = accountId;
        }

        public string GetUrl => $"api/cohorts?accountId={_accountId}";
    }
}
