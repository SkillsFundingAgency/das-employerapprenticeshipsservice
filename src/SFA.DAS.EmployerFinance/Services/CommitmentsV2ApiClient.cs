using Microsoft.Azure.Services.AppAuthentication;
using Newtonsoft.Json;
using SFA.DAS.Authentication.Extensions.Legacy;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.NLog.Logger;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Services
{
    public class CommitmentsV2ApiClient : ApiClientBase, ICommitmentsV2ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly CommitmentsApiV2ClientConfiguration _config;
        private readonly ILog _logger;

        public CommitmentsV2ApiClient(HttpClient httpClient, CommitmentsApiV2ClientConfiguration config, ILog logger) : base(httpClient)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        public async Task<GetApprenticeshipResponse> GetApprenticeship(long apprenticeshipId)
        {
            var url = $"{BaseUrl()}api/apprenticeships/{apprenticeshipId}";
            _logger.Info($"EmployerFinance Services Getting GetApprenticeship {url}");
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            await AddAuthenticationHeader(requestMessage);
            
            var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);            

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            _logger.Info($"EmployerFinance Services received response for GetApprenticeship {url}");
            return JsonConvert.DeserializeObject<GetApprenticeshipResponse>(json);
        }

        public async Task<GetTransferRequestSummaryResponse> GetTransferRequests(long accountId)
        {
            var url = $"{BaseUrl()}api/accounts/{accountId}/transfers";
            _logger.Info($"Getting GetTransferRequests {url}");
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            await AddAuthenticationHeader(requestMessage);

            var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<GetTransferRequestSummaryResponse>(json);
        }

        private string BaseUrl()
        {
            if (_config.ApiBaseUrl.EndsWith("/"))
            {
                return _config.ApiBaseUrl;
            }

            return _config.ApiBaseUrl + "/";
        }

        private async Task AddAuthenticationHeader(HttpRequestMessage httpRequestMessage)
        {
            if (ConfigurationManager.AppSettings["EnvironmentName"].ToUpper() != "LOCAL")
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var accessToken = await azureServiceTokenProvider.GetAccessTokenAsync(_config.IdentifierUri);
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }
    }
}
