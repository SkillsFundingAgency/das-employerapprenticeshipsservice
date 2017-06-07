using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client
{
    public class AccountApiClient : IAccountApiClient
    {
        private readonly IAccountApiConfiguration _configuration;
        private readonly SecureHttpClient _httpClient;

        public AccountApiClient(IAccountApiConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new SecureHttpClient(configuration);
        }

        internal AccountApiClient(IAccountApiConfiguration configuration, SecureHttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<AccountDetailViewModel> GetAccount(string hashedAccountId)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts/{hashedAccountId}";

            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<AccountDetailViewModel>(json);
        }

        public async Task<AccountDetailViewModel> GetAccount(long accountId)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts/internal/{accountId}";

            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<AccountDetailViewModel>(json);
        }

        public async Task<PagedApiResponseViewModel<AccountWithBalanceViewModel>> GetPageOfAccounts(int pageNumber = 1, int pageSize = 1000, DateTime? toDate = null)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts?pageNumber={pageNumber}&pageSize={pageSize}";
            if (toDate.HasValue)
            {
                var formattedToDate = toDate.Value.ToString("yyyyMMdd");
                url += $"&toDate={formattedToDate}";
            }

            var json = await _httpClient.GetAsync(url);
            return JsonConvert.DeserializeObject<PagedApiResponseViewModel<AccountWithBalanceViewModel>>(json);
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
