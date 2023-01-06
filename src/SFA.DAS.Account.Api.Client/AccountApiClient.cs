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

        public async Task<EmployerAgreementView> GetEmployerAgreement(string accountId, string legalEntityId, string agreementId)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts/{accountId}/legalEntities/{legalEntityId}/agreements/{agreementId}/agreement";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<EmployerAgreementView>(json);
        }

        public async Task<ICollection<AccountDetailViewModel>> GetUserAccounts(string userId)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/user/{userId}/accounts";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<ICollection<AccountDetailViewModel>>(json);
        }

        public Task Ping()
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/ping";

            return _httpClient.GetAsync(url);
        }

        private string GetBaseUrl()
        {
            return _configuration.ApiBaseUrl.EndsWith("/")
                ? _configuration.ApiBaseUrl
                : _configuration.ApiBaseUrl + "/";
        }
    }
}
