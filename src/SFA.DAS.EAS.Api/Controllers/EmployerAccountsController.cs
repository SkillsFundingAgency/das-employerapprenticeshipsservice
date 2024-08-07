﻿using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EAS.Account.Api.Authorization;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

namespace SFA.DAS.EAS.Account.Api.Controllers;

[ApiController]
[Route("api/accounts")]
public class EmployerAccountsController : ControllerBase
{
    private readonly AccountsOrchestrator _orchestrator;
    private readonly IEmployerAccountsApiService _apiService;

    public EmployerAccountsController(AccountsOrchestrator orchestrator, IEmployerAccountsApiService apiService)
    {
        _orchestrator = orchestrator;
        _apiService = apiService;
    }

    [Authorize(Policy = ApiRoles.ReadAllEmployerAccountBalances)]
    [HttpGet(Name = "AccountsIndex")]   
    public async Task<ActionResult<Types.PagedApiResponseViewModel<Types.AccountWithBalanceViewModel>>> GetAccounts(string toDate = null, int pageSize = 1000, int pageNumber = 1)
    {
        var result = await _orchestrator.GetAllAccountsWithBalances(toDate, pageSize, pageNumber);
        
        if (result.Status == HttpStatusCode.OK)
        {
            result.Data.Data.ForEach(x => x.Href = Url.Link("GetAccount", new { hashedAccountId = x.HashedAccountId }));
            return Ok(result.Data);
        }
    
        return Conflict();
    }


    [Authorize(Policy = ApiRoles.ReadAllEmployerAccountBalances)]
    [HttpGet("{hashedAccountId}", Name = "GetAccount")]
    public async Task<ActionResult<Types.AccountDetailViewModel>> GetAccount(string hashedAccountId)
    {
        var result = await _orchestrator.GetAccount(hashedAccountId);

        if (result.Data == null)
        {
            return NotFound();
        }

        return Ok(result.Data);
    }

    [Authorize(Policy = ApiRoles.ReadAllEmployerAccountBalances)]
    [HttpGet("internal/{accountId}", Name = "GetAccountByInternalId")]
    public async Task<IActionResult> GetAccount(long accountId)
    {
        var result = await _orchestrator.GetAccount(accountId);

        if (result.Data == null)
        {
            return NotFound();
        }
     
        return Ok(result.Data);
    }

    [Authorize(Policy = ApiRoles.ReadAllAccountUsers)]
    [HttpGet("{hashedAccountId}/users", Name = "GetAccountUsers")]
    public async Task<IActionResult> GetAccountUsers(string hashedAccountId)
    {
        var redirectResponse = await _apiService.Redirect($"/api/accounts/{hashedAccountId}/users");
        return Content(redirectResponse.ToString(), "application/json");
    }

    [Authorize(Policy = ApiRoles.ReadAllAccountUsers)]
    [HttpGet("internal/{accountId}/users", Name = "GetAccountUsersByInternalAccountId")]
    public async Task<IActionResult> GetAccountUsers(long accountId)
    {
        var redirectResponse = await _apiService.Redirect($"/api/accounts/internal/{accountId}/users");
        return Content(redirectResponse.ToString(), "application/json");
    }
}
