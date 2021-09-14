using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Interfaces.OuterApi;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests
{
    public class GetApplicationsRequest : IGetApiRequest
    {
        private readonly long _accountId;
        private readonly int _pageSize;

        public GetApplicationsRequest(long accountId, int pageSize)
        {
            _accountId = accountId;
            _pageSize = pageSize;
        }

        public string GetUrl => $"api/apprenticeships?accountId={_accountId}&PageItemCount={_pageSize}";
    }
}
