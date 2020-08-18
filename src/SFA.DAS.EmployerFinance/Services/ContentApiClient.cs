
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
    public class ContentApiClient : ApiClientBase, IContentApiClient
    {
        private readonly string ApiBaseUrl;

        public ContentApiClient(HttpClient client, IContentClientApiConfiguration configuration) : base(client) 
        {
            ApiBaseUrl = configuration.ApiBaseUrl.EndsWith("/")
                ? configuration.ApiBaseUrl
                : configuration.ApiBaseUrl + "/";
        }

        public async Task<string> Get(string type, string applicationId)
        {
            var uri = $"{ApiBaseUrl}api/content?applicationId={applicationId}&type={type}";
            
            return await GetAsync(uri); 
        }
    }
}
