using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Configuration;

namespace SFA.DAS.EmployerFinance.Infrastructure
{
    public class CommitmentsApiClient : ApiClient
    {
        public CommitmentsApiClient(HttpClient httpClient, EmployerFinanceConfiguration configuration) : base(httpClient)
        {
            httpClient.BaseAddress = new Uri(configuration.EmployerCommitmentsBaseUrl);
        }

        protected override void AddHeaders()
        {
            
        }
    }
}
