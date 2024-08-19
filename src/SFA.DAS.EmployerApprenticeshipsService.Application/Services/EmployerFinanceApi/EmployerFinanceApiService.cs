using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Http;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Transfers;

namespace SFA.DAS.EAS.Application.Services.EmployerFinanceApi;

public class EmployerFinanceApiService : ApiClientService, IEmployerFinanceApiService
{
    private readonly ILogger<EmployerFinanceApiService> _logger;

    public EmployerFinanceApiService(
        HttpClient httpClient,
        ILogger<EmployerFinanceApiService> logger,
        AzureServiceTokenProvider<EmployerFinanceApiConfiguration> tokenGenerator,
        EmployerFinanceApiConfiguration configuration)
        : base(httpClient, tokenGenerator)
    {
        _logger = logger;
        httpClient.BaseAddress = new Uri(configuration.ApiBaseUrl);
    }

    public Task<List<LevyDeclarationViewModel>> GetLevyDeclarations(string hashedAccountId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting EmployerFinanceApiService: GetLevyDeclarations {HashedAccountId}", hashedAccountId);

        var url = $"api/accounts/{hashedAccountId}/levy";

        _logger.LogInformation("Getting EmployerFinanceApiService : GetLevyDeclarations url : {Url}", url);

        return GetResponse<List<LevyDeclarationViewModel>>(url, cancellationToken: cancellationToken);
    }

    public Task<List<LevyDeclarationViewModel>> GetLevyForPeriod(string hashedAccountId, string payrollYear, short payrollMonth, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting EmployerFinanceApiService: GetLevyForPeriod {HashedAccountId} year: {PayrollYear} month: {PayrollMonth}", hashedAccountId, payrollYear, payrollMonth);

        var url = $"api/accounts/{hashedAccountId}/levy/{payrollYear}/{payrollMonth}";

        _logger.LogInformation("Getting EmployerFinanceApiService : GetLevyForPeriod url : {Url}", url);

        return GetResponse<List<LevyDeclarationViewModel>>(url, cancellationToken: cancellationToken);
    }

    public Task<TransactionsViewModel> GetTransactions(string accountId, int year, int month, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting EmployerFinanceApiService: GetTransactions {AccountId} year : {Year} month : {Month}", accountId, year, month);

        var url = $"api/accounts/{accountId}/transactions/{year}/{month}";

        _logger.LogInformation("Getting EmployerFinanceApiService : GetTransactions url : {Url}", url);

        return GetResponse<TransactionsViewModel>(url, cancellationToken: cancellationToken);
    }

    public Task<List<TransactionSummaryViewModel>> GetTransactionSummary(string accountId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting EmployerFinanceApiService: GetTransactionSummary {AccountId}", accountId);

        var url = $"api/accounts/{accountId}/transactions";

        _logger.LogInformation("Getting EmployerFinanceApiService : GetTransactionSummary url : {Url}", url);

        return GetResponse<List<TransactionSummaryViewModel>>(url, cancellationToken: cancellationToken);
    }

    public Task<TotalPaymentsModel> GetStatistics(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting EmployerFinanceApiService : statistics");

        return GetResponse<TotalPaymentsModel>("/api/financestatistics", cancellationToken: cancellationToken);
    }

    public async Task<List<AccountBalance>> GetAccountBalances(List<string> accountIds, CancellationToken cancellationToken = default)
    {
        const string url = "api/accounts/balances";
        
        var data = JsonConvert.SerializeObject(accountIds);
        using var stringContent = new StringContent(data, Encoding.UTF8, "application/json");

        _logger.LogInformation("Getting EmployerFinanceApiService : GetAccountBalances url : {Url}", url);
        _logger.LogInformation("stringContent {StringContent}", stringContent);

        // This uses a POST instead of a get due to the potential volume of accountId's being passed is too much for a GET request.
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = stringContent;
        
        await AddAuthenticationHeader(request);
        
        using var response = await Client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonConvert.DeserializeObject<List<AccountBalance>>(content);
    }

    public Task<TransferAllowance> GetTransferAllowance(string hashedAccountId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting EmployerFinanceApiService : GetTransferAllowance {HashedAccountId}", hashedAccountId);

        var url = $"api/accounts/{hashedAccountId}/transferAllowance";

        _logger.LogInformation("Getting EmployerFinanceApiService : GetTransferAllowance url : {Url}", url);

        return GetResponse<TransferAllowance>(url, cancellationToken: cancellationToken);
    }

    public Task<dynamic> Redirect(string url, CancellationToken cancellationToken = default)
    {
        return GetResponse<dynamic>(url, cancellationToken: cancellationToken);
    }
}