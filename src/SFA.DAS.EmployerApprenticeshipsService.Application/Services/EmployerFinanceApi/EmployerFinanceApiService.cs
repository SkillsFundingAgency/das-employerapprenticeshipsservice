using SFA.DAS.NLog.Logger;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi.Http;
using System.Net.Http;
using SFA.DAS.EAS.Application.Http;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Account.Api.Types;
using System.Text;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Application.Queries.GetTransferAllowance;

namespace SFA.DAS.EAS.Application.Services.EmployerFinanceApi
{
    public class EmployerFinanceApiService : IEmployerFinanceApiService
    {
        private readonly ILog _log;
        private readonly HttpClient _httpClient;

        public EmployerFinanceApiService(IEmployerFinanceApiHttpClientFactory employerFinanceApiHttpClientFactory, ILog log)
        {
            _httpClient = employerFinanceApiHttpClientFactory.CreateHttpClient();
            _log = log;
        }

        public async Task<List<LevyDeclarationViewModel>> GetLevyDeclarations(string hashedAccountId)
        {
            var url = $"api/accounts/{hashedAccountId}/levy";
            var response = await _httpClient.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return JsonConvert.DeserializeObject<List<LevyDeclarationViewModel>>(content);
        }

        public async Task<List<LevyDeclarationViewModel>> GetLevyForPeriod(string hashedAccountId, string payrollYear, short payrollMonth)
        {
            var url = $"api/accounts/{hashedAccountId}/levy/GetLevyForPeriod";
            var response = await _httpClient.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return JsonConvert.DeserializeObject<List<LevyDeclarationViewModel>>(content);
        }

        public async Task<TransactionsViewModel> GetTransactions(string accountId, int year, int month)
        {
            var url = $"api/accounts/{accountId}/transactions/{year}/{month}";
            var response = await _httpClient.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return JsonConvert.DeserializeObject<TransactionsViewModel>(content);
        }

        public async Task<List<TransactionSummaryViewModel>> GetTransactionSummary(string accountId)
        {
            var url = $"api/accounts/{accountId}/transactions";
            var response = await _httpClient.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return JsonConvert.DeserializeObject<List<TransactionSummaryViewModel>>(content);
        }

        public async Task<FinanceStatisticsViewModel> GetStatistics(CancellationToken cancellationToken = default)
        {
            _log.Info($"Getting statistics");

            var response = await _httpClient.GetAsync("/api/financestatistics", cancellationToken).ConfigureAwait(false);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return JsonConvert.DeserializeObject<FinanceStatisticsViewModel>(content);
        }

        public async Task<GetAccountBalancesResponse> GetAccountBalances(BulkAccountsRequest accountIds) // TODO : change to hashedAccountIds
        {
            var url = $"api/accounts/balances";
            var data = JsonConvert.SerializeObject(accountIds);
            var buffer = System.Text.Encoding.UTF8.GetBytes(data);
            var byteContent = new ByteArrayContent(buffer);
            var stringContent = new StringContent(data, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, stringContent);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return JsonConvert.DeserializeObject<GetAccountBalancesResponse>(content);
        }

        public async Task<GetTransferAllowanceResponse> GetTransferAllowance(long accountId) //TODO : change to hashedAccountId
        {
            var url = $"api/accounts/balances/{accountId}/transferAllowance";
            var response = await _httpClient.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return JsonConvert.DeserializeObject<GetTransferAllowanceResponse>(content);
        }
    }
}
