using SFA.DAS.EmployerFinance.Interfaces.OuterApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests
{
    public class GetApplicationsRequest : IGetApiRequest
    {
        private readonly long _accountId;

        public GetApplicationsRequest(long accountId)
        {
            _accountId = accountId;
        }

        public string GetUrl => $"Pledges?accountId={_accountId}";
    }
}
