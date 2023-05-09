using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi.Http;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.Http;

namespace SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

public class EmployerAccountsApiService : IEmployerAccountsApiService
{
    private readonly ILogger<EmployerAccountsApiService> _log;
    private readonly IRestHttpClient _httpClient;

    public EmployerAccountsApiService(
        IEmployerAccountsApiHttpClientFactory employerAccountsApiHttpClientFactory,
        ILogger<EmployerAccountsApiService> log)
    {
        _log = log;
        _httpClient = employerAccountsApiHttpClientFactory.CreateHttpClient();
    }

    public Task<Statistics> GetStatistics(CancellationToken cancellationToken = default)
    {
        _log.LogInformation("Getting statistics");

        return _httpClient.Get<Statistics>("/api/statistics", cancellationToken);
    }

    public Task<PagedApiResponseViewModel<AccountWithBalanceViewModel>> GetAccounts(string toDate = null, int pageSize = 1000, int pageNumber = 1, CancellationToken cancellationToken = default)
    {
        _log.LogInformation($"Getting paged accounts");

        return _httpClient.Get<PagedApiResponseViewModel<AccountWithBalanceViewModel>>($"/api/accounts?{(string.IsNullOrWhiteSpace(toDate) ? "" : "toDate=" + toDate + "&")}pageNumber={pageNumber}&pageSize={pageSize}", cancellationToken, cancellationToken);
    }

    public Task<AccountDetailViewModel> GetAccount(string hashedAccountId, CancellationToken cancellationToken = default)
    {
        _log.LogInformation("Getting paged accounts");

        return _httpClient.Get<AccountDetailViewModel>($"/api/accounts/{hashedAccountId}", cancellationToken);
    }

    public Task<string> Redirect(string url, CancellationToken cancellationToken = default)
    {
        return _httpClient.Get<string>(url, cancellationToken);
    }
}
