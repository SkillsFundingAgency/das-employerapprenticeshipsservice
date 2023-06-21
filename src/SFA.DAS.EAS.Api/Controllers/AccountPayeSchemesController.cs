using System;
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

    [Route("", Name = "GetPayeSchemes")]
    [Authorize(Policy = ApiRoles.ReadAllEmployerAccountBalances)]
    [HttpGet]
    public async Task<IActionResult> GetPayeSchemes([FromRoute] string hashedAccountId)
    {
        var redirectResponse = await _apiService.Redirect($"/api/accounts/{hashedAccountId}/payeschemes");
        return Content(redirectResponse.ToString(), "application/json");
    }

    [Route("scheme", Name = "GetPayeScheme")]
    [Authorize(Policy = ApiRoles.ReadAllEmployerAccountBalances)]
    [HttpGet]
    public async Task<IActionResult> GetPayeScheme([FromRoute] string hashedAccountId, [FromQuery] string payeSchemeRef)
    {
        var redirectResponse = await _apiService.Redirect($"/api/accounts/{hashedAccountId}/payeschemes/scheme?payeSchemeRef={Uri.EscapeDataString(payeSchemeRef)}");
        return Content(redirectResponse.ToString(), "application/json");
    }
}