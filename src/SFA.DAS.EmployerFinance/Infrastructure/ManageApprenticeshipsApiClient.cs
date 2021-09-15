using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Configuration;

namespace SFA.DAS.EmployerFinance.Infrastructure
{
    public class ManageApprenticeshipsApiClient : ApiClient
    {
        private readonly ManageApprenticeshipsOuterApiConfiguration _config;

        public ManageApprenticeshipsApiClient(HttpClient httpClient, ManageApprenticeshipsOuterApiConfiguration options) : base(httpClient)
        {
            _config = options;
            httpClient.BaseAddress = new Uri(_config.BaseUrl);
        }

        protected override void AddHeaders()
        {
            HttpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _config.Key);
            HttpClient.DefaultRequestHeaders.Add("X-Version", "1");
        }
    }
}
