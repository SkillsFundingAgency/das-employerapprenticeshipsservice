﻿using Microsoft.Azure.Services.AppAuthentication;
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
            var url = $"{BaseUrl()}api/apprenticeships/{apprenticeshipId}";
            _logger.LogInformation($"Getting GetApprenticeship {url}");
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            await AddAuthenticationHeader(requestMessage);
            
            var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<GetApprenticeshipResponse>(json);
        }

        public async Task<GetApprenticeshipsResponse> GetApprenticeships(GetApprenticeshipsRequest request)
        {
            var url = $"{BaseUrl()}api/apprenticeships/?accountId={request.AccountId}&reverseSort={request.ReverseSort}{request.SortField}{request.SortField}{request.SearchTerm}";
            _logger.LogInformation($"Getting GetApprenticeships {url}");
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            await AddAuthenticationHeader(requestMessage);            

            var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<GetApprenticeshipsResponse>(json);
        }

        public async Task<GetCohortsResponse> GetCohorts(GetCohortsRequest request)
        {
            var url = $"{BaseUrl()}api/cohorts/?accountId={request.AccountId}";
            _logger.LogInformation($"Getting GetCohorts {url}");
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            await AddAuthenticationHeader(requestMessage);

            var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<GetCohortsResponse>(json);
        }

        public async Task<GetDraftApprenticeshipsResponse> GetDraftApprenticeships(long cohortId)
        {
            var url = $"{BaseUrl()}api/cohorts/{cohortId}/draft-apprenticeships";
            _logger.LogInformation($"Getting GetDraftApprenticeships {url}");
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            await AddAuthenticationHeader(requestMessage);

            var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<GetDraftApprenticeshipsResponse>(json);
        }

        public async Task<GetApprenticeshipStatusSummaryResponse> GetEmployerAccountSummary(long accountId)
        {
            var url = $"{BaseUrl()}api/accounts/{accountId}/summary";
            _logger.LogInformation($"Getting GetEmployerAccountSummary {url}");
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            await AddAuthenticationHeader(requestMessage);
            
            var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<GetApprenticeshipStatusSummaryResponse>(json);
        }

        public async Task<GetTransferRequestSummaryResponse> GetTransferRequests(long accountId)
        {
            var url = $"{BaseUrl()}api/accounts/{accountId}/transfers";
            _logger.LogInformation($"Getting GetTransferRequests {url}");
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
