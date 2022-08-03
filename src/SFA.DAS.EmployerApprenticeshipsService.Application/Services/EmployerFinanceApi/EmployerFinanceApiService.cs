using SFA.DAS.NLog.Logger;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi.Http;
using System.Net.Http;
using SFA.DAS.EAS.Application.Http;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;

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

        public async Task<ICollection<Finance.Api.Types.LevyDeclarationViewModel>> GetLevyDeclarations(string hashedAccountId)
        {            
            var url = $"api/accounts/{hashedAccountId}/levy";
            var response = await _httpClient.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return JsonConvert.DeserializeObject<List<Finance.Api.Types.LevyDeclarationViewModel>>(content);
        }

        public async Task<ICollection<Finance.Api.Types.LevyDeclarationViewModel>> GetLevyForPeriod(string hashedAccountId, string payrollYear, short payrollMonth)
        {   
            var url = $"api/accounts/{hashedAccountId}/levy/GetLevyForPeriod";
            var response = await _httpClient.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return JsonConvert.DeserializeObject<List<Finance.Api.Types.LevyDeclarationViewModel>>(content);
        }

        public async Task<Finance.Api.Types.TransactionsViewModel> GetTransactions(string accountId, int year, int month)
        {            
            var url = $"api/accounts/{accountId}/transactions/{year}/{month}";
            var response = await _httpClient.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return JsonConvert.DeserializeObject<Finance.Api.Types.TransactionsViewModel>(content);
        }

        public async Task<ICollection<Finance.Api.Types.TransactionSummaryViewModel>> GetTransactionSummary(string accountId)
        {   
            var url = $"api/accounts/{accountId}/transactions";
            var response = await _httpClient.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return JsonConvert.DeserializeObject<ICollection<Finance.Api.Types.TransactionSummaryViewModel>>(content);
        }

        public async Task<Finance.Api.Types.FinanceStatisticsViewModel> GetStatistics(CancellationToken cancellationToken = default)
        {
            _log.Info($"Getting statistics");

            var response = await _httpClient.GetAsync("/api/financestatistics", cancellationToken).ConfigureAwait(false);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return JsonConvert.DeserializeObject<Finance.Api.Types.FinanceStatisticsViewModel>(content);
        }

        public async Task<GetAccountBalancesResponse> GetAccountBalances(List<long> accountIds)
        {
            var url = $"api/accounts/balances?accountIds={accountIds}";
            var response = await _httpClient.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new RestHttpClientException(response, content);

            return JsonConvert.DeserializeObject<GetAccountBalancesResponse>(content);
        }
    }
}
