
using SFA.DAS.Authentication.Extensions.Legacy;
using SFA.DAS.EmployerFinance.Interfaces;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using NLog.Internal;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace SFA.DAS.EmployerFinance.Services
{
    public class ContentApiClient : ApiClientBase, IContentApiClient
    {
        private readonly string _apiBaseUrl;
        private readonly string _identifierUri;
        private readonly HttpClient _client;

        public ContentApiClient(HttpClient client, IContentClientApiConfiguration configuration) : base(client) 
        {
            _apiBaseUrl = configuration.ApiBaseUrl.EndsWith("/")
                ? configuration.ApiBaseUrl
                : configuration.ApiBaseUrl + "/";

            _identifierUri = configuration.IdentifierUri;
            _client = client;
        }

        private async Task AddAuthenticationHeader()
        {
            if (ConfigurationManager.AppSettings["EnvironmentName"].ToUpper() != "LOCAL")
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var accessToken = await azureServiceTokenProvider.GetAccessTokenAsync(_identifierUri);

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }

        public async Task<string> Get(string type, string applicationId)
        {
            await AddAuthenticationHeader();

            var uri = $"{_apiBaseUrl}api/content?applicationId={applicationId}&type={type}";
            
            return await GetAsync(uri); 
        }
    }
}
