using SFA.DAS.EmployerFinance.Api.Client;
using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SFA.DAS.EAS.Application.Services.EmployerFinanceApi
{
    public class EmployerFinanceApiService : IEmployerFinanceApiService
    {
        private readonly ILog _log;
        private readonly IEmployerFinanceApiClientConfiguration _configuration;
        private readonly SecureHttpClient _httpClient;

        public EmployerFinanceApiService(IEmployerFinanceApiClientConfiguration configuration, ILog log)
        {
            _configuration = configuration;
            _log = log;
            _httpClient = new SecureHttpClient(configuration);
        }
     
        public async Task<ICollection<Finance.Api.Types.LevyDeclarationViewModel>> GetLevyDeclarations(string hashedAccountId)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts/{hashedAccountId}/levy";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<List<Finance.Api.Types.LevyDeclarationViewModel>>(json);
        }

        public async Task<ICollection<Finance.Api.Types.LevyDeclarationViewModel>> GetLevyForPeriod(string hashedAccountId, string payrollYear, short payrollMonth)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts/{hashedAccountId}/levy/GetLevyForPeriod";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<List<Finance.Api.Types.LevyDeclarationViewModel>>(json);
        }

        public async Task<Finance.Api.Types.Statistics> GetStatistics(CancellationToken cancellationToken = default)
        {
            _log.Info($"Getting statistics");

            var response = await _httpClient.GetAsync("/api/statistics", cancellationToken).ConfigureAwait(false);
           
            return JsonConvert.DeserializeObject<Finance.Api.Types.Statistics>(response);
        }

        public async Task<Finance.Api.Types.TransactionsViewModel> GetTransactions(string accountId, int year, int month)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts/{accountId}/transactions/{year}/{month}";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<Finance.Api.Types.TransactionsViewModel>(json);
        }

        public async Task<ICollection<Finance.Api.Types.TransactionSummaryViewModel>> GetTransactionSummary(string accountId)
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}api/accounts/{accountId}/transactions";
            var json = await _httpClient.GetAsync(url);

            return JsonConvert.DeserializeObject<ICollection<Finance.Api.Types.TransactionSummaryViewModel>>(json);
        }

        private string GetBaseUrl()
        {
            return _configuration.ApiBaseUrl.Trim('/');
        }
    }
}
