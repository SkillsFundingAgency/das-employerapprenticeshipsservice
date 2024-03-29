﻿using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Http;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

public class EmployerAccountsApiService : ApiClientService, IEmployerAccountsApiService
{
    private readonly ILogger<EmployerAccountsApiService> _logger;

    public EmployerAccountsApiService(HttpClient httpClient, 
        ILogger<EmployerAccountsApiService> logger,
        ManagedIdentityTokenGenerator<EmployerAccountsApiConfiguration> tokenGenerator,
        EmployerAccountsApiConfiguration configuration) : base(httpClient, tokenGenerator)
    {
        _logger = logger;
        httpClient.BaseAddress = new Uri(configuration.ApiBaseUrl);
    }

    public Task<Statistics> GetStatistics(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting statistics");
        
        return GetResponse<Statistics>("/api/statistics", cancellationToken: cancellationToken);
    }

    public Task<PagedApiResponseViewModel<AccountWithBalanceViewModel>> GetAccounts(string toDate = null, int pageSize = 1000, int pageNumber = 1, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Getting paged accounts");

        return GetResponse<PagedApiResponseViewModel<AccountWithBalanceViewModel>>($"/api/accounts?{(string.IsNullOrWhiteSpace(toDate) ? "" : "toDate=" + toDate + "&")}pageNumber={pageNumber}&pageSize={pageSize}", cancellationToken: cancellationToken);
    }

    public Task<AccountDetailViewModel> GetAccount(string hashedAccountId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting paged accounts");

        return GetResponse<AccountDetailViewModel>($"/api/accounts/{hashedAccountId}", cancellationToken: cancellationToken);
    }

    public Task<dynamic> Redirect(string url, CancellationToken cancellationToken = default)
    {
        return GetResponse<dynamic>(url, cancellationToken: cancellationToken);
    }
}
