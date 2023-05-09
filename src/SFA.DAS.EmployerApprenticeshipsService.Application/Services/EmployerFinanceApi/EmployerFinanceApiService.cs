using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi.Http;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.Http;

namespace SFA.DAS.EAS.Application.Services.EmployerFinanceApi;

public class EmployerFinanceApiService : IEmployerFinanceApiService
{
    private readonly ILogger<EmployerFinanceApiService> _log;
    private readonly IRestHttpClient _httpClient;

    public EmployerFinanceApiService(IEmployerFinanceApiHttpClientFactory employerFinanceApiHttpClientFactory,
        ILogger<EmployerFinanceApiService> log)
    {
        _httpClient = employerFinanceApiHttpClientFactory.CreateHttpClient();
        _log = log;
    }

    public Task<List<LevyDeclarationViewModel>> GetLevyDeclarations(string hashedAccountId, CancellationToken cancellationToken = default)
    {
        _log.LogInformation("Getting  EmployerFinanceApiService: GetLevyDeclarations {HashedAccountId}", hashedAccountId);

        var url = $"api/accounts/{hashedAccountId}/levy";

        _log.LogInformation("Getting EmployerFinanceApiService : GetLevyDeclarations url : {Url}", url);

        return _httpClient.Get<List<LevyDeclarationViewModel>>(url, cancellationToken);
    }

    public Task<List<LevyDeclarationViewModel>> GetLevyForPeriod(string hashedAccountId, string payrollYear, short payrollMonth, CancellationToken cancellationToken = default)
    {
        _log.LogInformation("Getting EmployerFinanceApiService: GetLevyForPeriod {HashedAccountId} year: {PayrollYear} month: {PayrollMonth}", hashedAccountId, payrollYear, payrollMonth);

        var url = $"api/accounts/{hashedAccountId}/levy/{payrollYear}/{payrollMonth}";

        _log.LogInformation("Getting EmployerFinanceApiService : GetLevyForPeriod url : {Url}", url);

        return _httpClient.Get<List<LevyDeclarationViewModel>>(url, cancellationToken);
    }

    public Task<TransactionsViewModel> GetTransactions(string accountId, int year, int month, CancellationToken cancellationToken = default)
    {
        _log.LogInformation("Getting  EmployerFinanceApiService: GetTransactions {AccountId} year : {Year} month : {Month}", accountId, year, month);

        var url = $"api/accounts/{accountId}/transactions/{year}/{month}";

        _log.LogInformation("Getting EmployerFinanceApiService : GetTransactions url : {Url}", url);

        return _httpClient.Get<TransactionsViewModel>(url, cancellationToken);
    }

    public Task<List<TransactionSummaryViewModel>> GetTransactionSummary(string accountId, CancellationToken cancellationToken = default)
    {
        _log.LogInformation("Getting  EmployerFinanceApiService: GetTransactionSummary {AccountId}", accountId);

        var url = $"api/accounts/{accountId}/transactions";

        _log.LogInformation("Getting EmployerFinanceApiService : GetTransactionSummary url : {Url}", url);

        return _httpClient.Get<List<TransactionSummaryViewModel>>(url, cancellationToken);
    }

    public Task<TotalPaymentsModel> GetStatistics(CancellationToken cancellationToken = default)
    {
        _log.LogInformation("Getting EmployerFinanceApiService : statistics");

        return _httpClient.Get<TotalPaymentsModel>("/api/financestatistics", cancellationToken);
    }

    public Task<List<AccountBalance>> GetAccountBalances(List<string> accountIds, CancellationToken cancellationToken = default)
    {
        _log.LogInformation($"Getting EmployerFinanceApiService : GetAccountBalances");

        var url = "api/accounts/balances";
        var data = JsonConvert.SerializeObject(accountIds);
        var stringContent = new StringContent(data, System.Text.Encoding.UTF8, "application/json");

        _log.LogInformation("Getting EmployerFinanceApiService : GetAccountBalances url : {Url}", url);
        _log.LogInformation("stringContent {StringContent}", stringContent);

        return _httpClient.Get<List<AccountBalance>>(url, stringContent, cancellationToken);
    }

    public Task<TransferAllowance> GetTransferAllowance(string hashedAccountId, CancellationToken cancellationToken = default)
    {
        _log.LogInformation("Getting EmployerFinanceApiService : GetTransferAllowance {HashedAccountId}", hashedAccountId);

        var url = $"api/accounts/{hashedAccountId}/transferAllowance";

        _log.LogInformation("Getting EmployerFinanceApiService : GetTransferAllowance url : {Url}", url);

        return _httpClient.Get<TransferAllowance>(url, cancellationToken);
    }

    public Task<string> Redirect(string url, CancellationToken cancellationToken = default)
    {
        return _httpClient.Get<string>(url, cancellationToken);
    }
}