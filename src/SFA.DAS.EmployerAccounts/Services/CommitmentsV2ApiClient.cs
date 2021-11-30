using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Authentication.Extensions.Legacy;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class CommitmentsV2ApiClient : ApiClientBase, ICommitmentsV2ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly CommitmentsApiV2ClientConfiguration _config;
        private readonly ILogger<CommitmentsV2ApiClient> _logger;        

        public CommitmentsV2ApiClient(HttpClient httpClient, CommitmentsApiV2ClientConfiguration config, ILogger<CommitmentsV2ApiClient> logger) : base(httpClient)
        {
            _httpClient = httpClient;
            _config = config;            
            _logger = logger;
        }

        public async Task<GetApprenticeshipResponse> GetApprenticeship(long apprenticeshipId)
        {
            await AddAuthenticationHeader();

            var url = $"{BaseUrl()}api/apprenticeships/{apprenticeshipId}";
            _logger.LogInformation($"Getting GetApprenticeship {url}");
            var response = JsonConvert.DeserializeObject<GetApprenticeshipResponse>(await GetAsync(url));
            return response;
        }

        public async Task<GetApprenticeshipsResponse> GetApprenticeships(GetApprenticeshipsRequest request)
        {
            await AddAuthenticationHeader();

            var url = $"{BaseUrl()}api/apprenticeships/?accountId={request.AccountId}&reverseSort={request.ReverseSort}{request.SortField}{request.SortField}{request.SearchTerm}";
            _logger.LogInformation($"Getting GetApprenticeships {url}");
            var response = JsonConvert.DeserializeObject<GetApprenticeshipsResponse>(await GetAsync(url));
            return response;
        }

        public async Task<GetCohortsResponse> GetCohorts(GetCohortsRequest request)
        {
            await AddAuthenticationHeader();

            var url = $"{BaseUrl()}api/cohorts/{request}";
            _logger.LogInformation($"Getting GetCohorts {url}");
            var response = JsonConvert.DeserializeObject<GetCohortsResponse>(await GetAsync(url));
            return response;
        }

        public async Task<GetDraftApprenticeshipsResponse> GetDraftApprenticeships(long cohortId)
        {
            await AddAuthenticationHeader();

            var url = $"{BaseUrl()}api/cohorts/{cohortId}/draft-apprenticeships";
            _logger.LogInformation($"Getting GetDraftApprenticeships {url}");
            var response = JsonConvert.DeserializeObject<GetDraftApprenticeshipsResponse>(await GetAsync(url));
            return response;
        }

        public async Task<GetApprenticeshipStatusSummaryResponse> GetEmployerAccountSummary(long employerAccountId)
        {
            await AddAuthenticationHeader();

            var url = $"{BaseUrl()}api/accounts/{employerAccountId}/employer-account-summary";
            _logger.LogInformation($"Getting GetEmployerAccountSummary {url}");
            var response = JsonConvert.DeserializeObject<GetApprenticeshipStatusSummaryResponse>(await GetAsync(url));
            return response;
        }

        public async Task<GetTransferRequestSummaryResponse> GetTransferRequests(long accountId)
        {
            await AddAuthenticationHeader();

            var url = $"{BaseUrl()}api/accounts/{accountId}/transfers";
            _logger.LogInformation($"Getting GetTransferRequests {url}");
            var response = JsonConvert.DeserializeObject<GetTransferRequestSummaryResponse>(await GetAsync(url));
            return response;
        }

        private string BaseUrl()
        {
            if (_config.ApiBaseUrl.EndsWith("/"))
            {
                return _config.ApiBaseUrl;
            }

            return _config.ApiBaseUrl + "/";
        }

        private async Task AddAuthenticationHeader()
        {
            if (ConfigurationManager.AppSettings["EnvironmentName"].ToUpper() != "LOCAL")
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var accessToken = await azureServiceTokenProvider.GetAccessTokenAsync(_config.IdentifierUri);

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);            
            }
        }
    }
}
