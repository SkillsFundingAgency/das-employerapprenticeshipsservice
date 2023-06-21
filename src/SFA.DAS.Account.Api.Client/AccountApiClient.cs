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

        public AccountApiClient(IAccountApiConfiguration configuration, SecureHttpClient httpClient)
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

        public async Task<EmployerAgreementView> GetEmployerAgreement(string accountId, string legalEntityId, string agreementId)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts/{accountId}/legalEntities/{legalEntityId}/agreements/{agreementId}/agreement";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<EmployerAgreementView>(json);
        }

        public async Task<ICollection<ResourceViewModel>> GetLegalEntitiesConnectedToAccount(string accountId)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts/{accountId}/legalentities";
            
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<List<ResourceViewModel>>(json);
        }

        public async Task<ICollection<LegalEntityViewModel>> GetLegalEntityDetailsConnectedToAccount(string accountId)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts/{accountId}/legalentities?includeDetails=true";
            
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<List<LegalEntityViewModel>>(json);
        }

        public async Task<LegalEntityViewModel> GetLegalEntity(string accountId, long id)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts/{accountId}/legalentities/{id}";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<LegalEntityViewModel>(json);
        }

        public async Task<ICollection<LevyDeclarationViewModel>> GetLevyDeclarations(string accountId)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts/{accountId}/levy";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<List<LevyDeclarationViewModel>>(json);
        }

        public async Task<PagedApiResponseViewModel<AccountLegalEntityViewModel>> GetPageOfAccountLegalEntities(int pageNumber = 1, int pageSize = 1000)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accountlegalentities?pageNumber={pageNumber}&pageSize={pageSize}";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<PagedApiResponseViewModel<AccountLegalEntityViewModel>>(json);
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

        public async Task<ICollection<ResourceViewModel>> GetPayeSchemesConnectedToAccount(string accountId)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts/{accountId}/payeschemes";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<List<ResourceViewModel>>(json);
        }

        public async Task<T> GetResource<T>(string uri)
        {
            var absoluteUri = new Uri(new Uri(GetBaseUrl()), uri);
            var json = await _httpClient.GetAsync(absoluteUri.ToString());

            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task<StatisticsViewModel> GetStatistics()
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/statistics";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<StatisticsViewModel>(json);
        }

        public async Task<TransactionsViewModel> GetTransactions(string accountId, int year, int month)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts/{accountId}/transactions/{year}/{month}";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<TransactionsViewModel>(json);
        }

        public async Task<ICollection<TransactionSummaryViewModel>> GetTransactionSummary(string accountId)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts/{accountId}/transactions";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<ICollection<TransactionSummaryViewModel>>(json);
        }

        public async Task<ICollection<TransferConnectionViewModel>> GetTransferConnections(string accountHashedId)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts/{accountHashedId}/transfers/connections";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<ICollection<TransferConnectionViewModel>>(json);
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
