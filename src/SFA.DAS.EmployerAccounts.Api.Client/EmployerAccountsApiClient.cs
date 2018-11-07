using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.LegalEntities;

namespace SFA.DAS.EmployerAccounts.Api.Client
{
    public class EmployerAccountsApiClient : IEmployerAccountsApiClient
    {
        private readonly IEmployerAccountsApiClientConfiguration _configuration;
        private readonly SecureHttpClient _httpClient;

        public EmployerAccountsApiClient(IEmployerAccountsApiClientConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new SecureHttpClient(configuration);
        }

        public EmployerAccountsApiClient(IEmployerAccountsApiClientConfiguration configuration, SecureHttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public Task HealthCheck()
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}/api/healthcheck";

            return _httpClient.GetAsync(url);
        }

        public async Task<ICollection<TeamMemberViewModel>> GetAccountUsers(string accountId)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts/{accountId}/users";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<ICollection<TeamMemberViewModel>>(json);
        }

        public async Task<ICollection<TeamMemberViewModel>> GetAccountUsers(long accountId)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts/internal/{accountId}/users";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<ICollection<TeamMemberViewModel>>(json);
        }

        public async Task<ICollection<AccountDetailViewModel>> GetUserAccounts(string userId)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/user/{userId}/accounts";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<ICollection<AccountDetailViewModel>>(json);
        }

        public async Task<ICollection<ResourceViewModel>> GetPayeSchemesConnectedToAccount(string accountId)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts/{accountId}/payeschemes";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<List<ResourceViewModel>>(json);
        }

        public async Task<ICollection<ResourceViewModel>> GetLegalEntitiesConnectedToAccount(string accountId)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts/{accountId}/legalentities";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<List<ResourceViewModel>>(json);
        }

        public async Task<LegalEntityViewModel> GetLegalEntity(string accountId, long id)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts/{accountId}/legalentities/{id}";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<LegalEntityViewModel>(json);
        }

        public async Task<PagedApiResponseViewModel<AccountLegalEntityViewModel>> GetPageOfAccountLegalEntities(int pageNumber = 1, int pageSize = 1000)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accountlegalentities?pageNumber={pageNumber}&pageSize={pageSize}";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<PagedApiResponseViewModel<AccountLegalEntityViewModel>>(json);
        }

        public async Task<T> GetResource<T>(string uri) where T : IAccountResource
        {
            var absoluteUri = new Uri(new Uri(GetBaseUrl()), uri);
            var json = await _httpClient.GetAsync(absoluteUri.ToString());

            return JsonConvert.DeserializeObject<T>(json);
        }

        private string GetBaseUrl()
        {
            return _configuration.ApiBaseUrl.EndsWith("/")
                ? _configuration.ApiBaseUrl
                : _configuration.ApiBaseUrl + "/";
        }
    }
}