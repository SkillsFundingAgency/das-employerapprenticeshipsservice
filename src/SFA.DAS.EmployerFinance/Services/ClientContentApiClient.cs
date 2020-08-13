
using SFA.DAS.Authentication.Extensions.Legacy;
using SFA.DAS.EmployerFinance.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Services
{
    public class ClientContentApiClient : ApiClientBase, IClientContentApiClient
    {
        private readonly string ApiBaseUrl;

        public ClientContentApiClient(HttpClient client, IContentClientApiConfiguration configuration) : base(client) 
        {
            ApiBaseUrl = configuration.ApiBaseUrl.EndsWith("/")
                ? configuration.ApiBaseUrl
                : configuration.ApiBaseUrl + "/";
        }

        public async Task<string> Get(string type, string applicationId)
        {
            string banner = string.Empty;
            switch (type)
            {
                case "banner":
                    var uri = $"{ApiBaseUrl}api/content?applicationId={applicationId}&type={type}";
                    banner = await GetAsync(uri);
                    break;
            }
            return banner;
        }
    }
}
