﻿using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EAS.Account.Api.Authorization;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

namespace SFA.DAS.EAS.Account.Api.Controllers;

[ApiController]
[Route("api/accounts/{hashedAccountId}/payeschemes")]
public class AccountPayeSchemesController : ControllerBase
{
    private readonly IEmployerAccountsApiService _apiService;

    public AccountPayeSchemesController(IEmployerAccountsApiService apiService)
    {
        _apiService = apiService;
    }

    [Authorize(Policy = ApiRoles.ReadAllEmployerAccountBalances)]
    [HttpGet(Name = "GetPayeSchemes")]
    public async Task<IActionResult> GetPayeSchemes(string hashedAccountId)
    {
        var redirectResponse = await _apiService.Redirect($"/api/accounts/{hashedAccountId}/payeschemes");
        return Content(redirectResponse.ToString(), "application/json");
    }

    [Authorize(Policy = ApiRoles.ReadAllEmployerAccountBalances)]
    [HttpGet("{payeSchemeRef}", Name = "GetPayeScheme")]
    public async Task<IActionResult> GetPayeScheme(string hashedAccountId, string payeSchemeRef)
    {
        var redirectResponse = await _apiService.Redirect($"/api/accounts/{hashedAccountId}/payeschemes/{WebUtility.UrlEncode(payeSchemeRef)}");
        return Content(redirectResponse.ToString(), "application/json");
    }
}