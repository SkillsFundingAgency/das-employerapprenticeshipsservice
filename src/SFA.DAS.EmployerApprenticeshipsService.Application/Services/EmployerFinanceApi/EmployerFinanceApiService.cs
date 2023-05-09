using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Http;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi.Http;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Transfers;

namespace SFA.DAS.EAS.Application.Services.EmployerFinanceApi;

public class EmployerFinanceApiService : ApiClientService, IEmployerFinanceApiService
{
    private readonly ILogger<EmployerFinanceApiService> _log;

    public EmployerFinanceApiService(IEmployerFinanceApiHttpClientFactory clientFactory, ILogger<EmployerFinanceApiService> log) : base(clientFactory.CreateHttpClient())
    {
        _log = log;
    }

    public Task<List<LevyDeclarationViewModel>> GetLevyDeclarations(string hashedAccountId, CancellationToken cancellationToken = default)
    {
        _log.LogInformation("Getting EmployerFinanceApiService: GetLevyDeclarations {HashedAccountId}", hashedAccountId);

        var url = $"api/accounts/{hashedAccountId}/levy";

        _log.LogInformation("Getting EmployerFinanceApiService : GetLevyDeclarations url : {Url}", url);

        return Get<List<LevyDeclarationViewModel>>(url, cancellationToken);
    }

    public Task<List<LevyDeclarationViewModel>> GetLevyForPeriod(string hashedAccountId, string payrollYear, short payrollMonth, CancellationToken cancellationToken = default)
    {
        _log.LogInformation("Getting EmployerFinanceApiService: GetLevyForPeriod {HashedAccountId} year: {PayrollYear} month: {PayrollMonth}", hashedAccountId, payrollYear, payrollMonth);

        var url = $"api/accounts/{hashedAccountId}/levy/{payrollYear}/{payrollMonth}";

        _log.LogInformation("Getting EmployerFinanceApiService : GetLevyForPeriod url : {Url}", url);

        return Get<List<LevyDeclarationViewModel>>(url, cancellationToken);
    }

    public Task<TransactionsViewModel> GetTransactions(string accountId, int year, int month, CancellationToken cancellationToken = default)
    {
        _log.LogInformation("Getting EmployerFinanceApiService: GetTransactions {AccountId} year : {Year} month : {Month}", accountId, year, month);

        var url = $"api/accounts/{accountId}/transactions/{year}/{month}";

        _log.LogInformation("Getting EmployerFinanceApiService : GetTransactions url : {Url}", url);

        return Get<TransactionsViewModel>(url, cancellationToken);
    }

    public Task<List<TransactionSummaryViewModel>> GetTransactionSummary(string accountId, CancellationToken cancellationToken = default)
    {
        _log.LogInformation("Getting EmployerFinanceApiService: GetTransactionSummary {AccountId}", accountId);

        var url = $"api/accounts/{accountId}/transactions";

        _log.LogInformation("Getting EmployerFinanceApiService : GetTransactionSummary url : {Url}", url);

        return Get<List<TransactionSummaryViewModel>>(url, cancellationToken);
    }

    public Task<TotalPaymentsModel> GetStatistics(CancellationToken cancellationToken = default)
    {
        _log.LogInformation("Getting EmployerFinanceApiService : statistics");

        return Get<TotalPaymentsModel>("/api/financestatistics", cancellationToken);
    }

    public async Task<List<AccountBalance>> GetAccountBalances(List<string> accountIds, CancellationToken cancellationToken = default)
    {
        var url = "api/accounts/balances";
        var data = JsonConvert.SerializeObject(accountIds);
        var stringContent = new StringContent(data, System.Text.Encoding.UTF8, "application/json");

        _log.LogInformation("Getting EmployerFinanceApiService : GetAccountBalances url : {Url}", url);
        _log.LogInformation("stringContent {StringContent}", stringContent);

        // This uses a POST instead of a get due to the potential volume of accountId's being passed is too much for a GET request.
        var response = await _httpClient.PostAsync(url, stringContent, cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw new RestHttpClientException(response, content);
        }

        return JsonConvert.DeserializeObject<List<AccountBalance>>(content);
    }

    public Task<TransferAllowance> GetTransferAllowance(string hashedAccountId, CancellationToken cancellationToken = default)
    {
        _log.LogInformation("Getting EmployerFinanceApiService : GetTransferAllowance {HashedAccountId}", hashedAccountId);

        var url = $"api/accounts/{hashedAccountId}/transferAllowance";

        _log.LogInformation("Getting EmployerFinanceApiService : GetTransferAllowance url : {Url}", url);

        return Get<TransferAllowance>(url, cancellationToken);
    }

    public Task<string> Redirect(string url, CancellationToken cancellationToken = default)
    {
        return Get<string>(url, cancellationToken);
    }
}