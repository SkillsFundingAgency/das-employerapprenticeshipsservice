using Newtonsoft.Json;
using SFA.DAS.EAS.Finance.Api.Types;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Api.Client
{
    public class EmployerFinanceApiClient : IEmployerFinanceApiClient
    {
        private readonly IEmployerFinanceApiClientConfiguration _configuration;
        private readonly SecureHttpClient _httpClient;

        public EmployerFinanceApiClient(IEmployerFinanceApiClientConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new SecureHttpClient(configuration);
        }

        public Task HealthCheck()
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}/api/healthcheck";

            return _httpClient.GetAsync(url);
        }

        public async Task<ICollection<LevyDeclarationViewModel>> GetLevyDeclarations(string hashedAccountId)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}/api/accounts/{hashedAccountId}/levy";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<List<LevyDeclarationViewModel>>(json);
        }
       
        public async Task<ICollection<LevyDeclarationViewModel>> GetLevyForPeriod(string hashedAccountId, string payrollYear, short payrollMonth)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}/api/accounts/{hashedAccountId}/levy/GetLevyForPeriod";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<List<LevyDeclarationViewModel>>(json);
        }

        public async Task<TransactionsViewModel> GetTransactions(string accountId, int year, int month)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}/api/accounts/{accountId}/transactions/{year}/{month}";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<TransactionsViewModel>(json);
        }

        public async Task<ICollection<TransactionSummaryViewModel>> GetTransactionSummary(string accountId)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}/api/accounts/{accountId}/transactions";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<ICollection<TransactionSummaryViewModel>>(json);
        }

        public async Task<FinanceStatisticsViewModel> GetFinanceStatistics()
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}/api/financestatistics";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<FinanceStatisticsViewModel>(json);
        }


        private string GetBaseUrl()
        {
            return _configuration.ApiBaseUrl.Trim('/');  //https://localhost:44366/api/accounts/103960/levy
        }

      
    }
}