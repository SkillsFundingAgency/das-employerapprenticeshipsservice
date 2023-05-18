using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi.Http;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

public class EmployerAccountsApiService : ApiClientService, IEmployerAccountsApiService
{
    private readonly ILogger<EmployerAccountsApiService> _log;

    public EmployerAccountsApiService(IEmployerAccountsApiHttpClientFactory clientFactory, ILogger<EmployerAccountsApiService> log) : base(clientFactory.CreateHttpClient())
    {
        _log = log;
    }

    public Task<Statistics> GetStatistics(CancellationToken cancellationToken = default)
    {
        _log.LogInformation("Getting statistics");

        return GetResponse<Statistics>("/api/statistics", cancellationToken);
    }

    public Task<PagedApiResponseViewModel<AccountWithBalanceViewModel>> GetAccounts(string toDate = null, int pageSize = 1000, int pageNumber = 1, CancellationToken cancellationToken = default)
    {
        _log.LogInformation($"Getting paged accounts");

        return GetResponse<PagedApiResponseViewModel<AccountWithBalanceViewModel>>($"/api/accounts?{(string.IsNullOrWhiteSpace(toDate) ? "" : "toDate=" + toDate + "&")}pageNumber={pageNumber}&pageSize={pageSize}", cancellationToken, cancellationToken);
    }

    public Task<AccountDetailViewModel> GetAccount(string hashedAccountId, CancellationToken cancellationToken = default)
    {
        _log.LogInformation("Getting paged accounts");

        return GetResponse<AccountDetailViewModel>($"/api/accounts/{hashedAccountId}", cancellationToken);
    }

    public Task<dynamic> Redirect(string url, CancellationToken cancellationToken = default)
    {
        return GetResponse<dynamic>(url, cancellationToken);
    }
}
