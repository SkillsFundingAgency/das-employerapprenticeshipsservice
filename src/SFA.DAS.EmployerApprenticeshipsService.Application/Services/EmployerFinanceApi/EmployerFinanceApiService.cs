using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Http;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi.Http;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.NLog.Logger;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EAS.Application.Services.EmployerFinanceApi
{
    public class EmployerFinanceApiService : IEmployerFinanceApiService
    {
        private readonly ILogger<EmployerFinanceApiService> _log;
        private readonly HttpClient _httpClient;

        public EmployerFinanceApiService(IEmployerFinanceApiHttpClientFactory employerFinanceApiHttpClientFactory, 
            ILogger<EmployerFinanceApiService> log)
        {
            _httpClient = employerFinanceApiHttpClientFactory.CreateHttpClient();
            _log = log;
        }

        public async Task<List<LevyDeclarationViewModel>> GetLevyDeclarations(string hashedAccountId)
        {
            _log.LogInformation($"Getting  EmployerFinanceApiService: GetLevyDeclarations {hashedAccountId}");

            var url = $"api/accounts/{hashedAccountId}/levy";
            var response = await _httpClient.GetAsync(url);

            _log.LogInformation($"Getting EmployerFinanceApiService : GetLevyDeclarations url : {url}");

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return JsonConvert.DeserializeObject<List<LevyDeclarationViewModel>>(content);
        }

        public async Task<List<LevyDeclarationViewModel>> GetLevyForPeriod(string hashedAccountId, string payrollYear, short payrollMonth)
        {
            _log.LogInformation($"Getting  EmployerFinanceApiService: GetLevyForPeriod {hashedAccountId} year: {payrollYear} month: {payrollMonth}");

            var url = $"api/accounts/{hashedAccountId}/levy/{payrollYear}/{payrollMonth}";
            var response = await _httpClient.GetAsync(url);

            _log.LogInformation($"Getting EmployerFinanceApiService : GetLevyForPeriod url : {url}");

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return JsonConvert.DeserializeObject<List<LevyDeclarationViewModel>>(content);
        }

        public async Task<TransactionsViewModel> GetTransactions(string accountId, int year, int month)
        {
            _log.LogInformation($"Getting  EmployerFinanceApiService: GetTransactions {accountId} year : {year} month : {month}");

            var url = $"api/accounts/{accountId}/transactions/{year}/{month}";
            var response = await _httpClient.GetAsync(url);

            _log.LogInformation($"Getting EmployerFinanceApiService : GetTransactions url : {url}");

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return JsonConvert.DeserializeObject<TransactionsViewModel>(content);
        }

        public async Task<List<TransactionSummaryViewModel>> GetTransactionSummary(string accountId)
        {
            _log.LogInformation($"Getting  EmployerFinanceApiService: GetTransactionSummary {accountId}");

            var url = $"api/accounts/{accountId}/transactions";
            var response = await _httpClient.GetAsync(url);

            _log.LogInformation($"Getting EmployerFinanceApiService : GetTransactionSummary url : {url}");

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return JsonConvert.DeserializeObject<List<TransactionSummaryViewModel>>(content);
        }

        public async Task<TotalPaymentsModel> GetStatistics(CancellationToken cancellationToken = default)
        {
            _log.LogInformation($"Getting EmployerFinanceApiService : statistics");

            var response = await _httpClient.GetAsync("/api/financestatistics", cancellationToken).ConfigureAwait(false);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return JsonConvert.DeserializeObject<TotalPaymentsModel>(content);
        }     

        public async Task<List<AccountBalance>> GetAccountBalances(List<string> accountIds)
        {
            _log.LogInformation($"Getting EmployerFinanceApiService : GetAccountBalances");

            var url = $"api/accounts/balances";
            var data = JsonConvert.SerializeObject(accountIds);
            var stringContent = new StringContent(data, Encoding.UTF8, "application/json");

            _log.LogInformation($"Getting EmployerFinanceApiService : GetAccountBalances url : {url}");
            _log.LogInformation($"stringContent {stringContent}");

            var response = await _httpClient.PostAsync(url, stringContent);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return JsonConvert.DeserializeObject<List<AccountBalance>>(content);
        }

        public async Task<TransferAllowance> GetTransferAllowance(string hashedAccountId)
        {
            _log.LogInformation($"Getting EmployerFinanceApiService : GetTransferAllowance {hashedAccountId}");

            var url = $"api/accounts/{hashedAccountId}/transferAllowance";
            var response = await _httpClient.GetAsync(url);

            _log.LogInformation($"Getting EmployerFinanceApiService : GetTransferAllowance url : {url}");

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return JsonConvert.DeserializeObject<TransferAllowance>(content);
        }
    
        public async Task<string> Redirect(string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return content;
        }
    }
}