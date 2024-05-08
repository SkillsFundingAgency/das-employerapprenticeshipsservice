using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EAS.Account.Api.Authorization;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.Encoding;

namespace SFA.DAS.EAS.Account.Api.Controllers;

[ApiController]
[Route("api/accounts/{hashedAccountId}/payeschemes")]
public class AccountPayeSchemesController : ControllerBase
{
    private readonly IEmployerAccountsApiService _apiService;
    private readonly IEncodingService _encodingService;

    public AccountPayeSchemesController(IEmployerAccountsApiService apiService, IEncodingService encodingService)
    {
        _apiService = apiService;
        _encodingService = encodingService;
    }

    [Route("", Name = "GetPayeSchemes")]
    [Authorize(Policy = ApiRoles.ReadAllEmployerAccountBalances)]
    [HttpGet]
    public async Task<IActionResult> GetPayeSchemes([FromRoute] string hashedAccountId)
    {
        var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);
        var redirectResponse = await _apiService.Redirect($"/api/accounts/{accountId}/payeschemes");
        return Content(redirectResponse.ToString(), "application/json");
    }

    [Route("scheme", Name = "GetPayeScheme")]
    [Authorize(Policy = ApiRoles.ReadAllEmployerAccountBalances)]
    [HttpGet]
    public async Task<IActionResult> GetPayeScheme([FromRoute] string hashedAccountId, [FromQuery] string payeSchemeRef)
    {
        var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);
        var redirectResponse = await _apiService.Redirect($"/api/accounts/{accountId}/payeschemes/scheme?payeSchemeRef={Uri.EscapeDataString(payeSchemeRef)}");
        return Content(redirectResponse.ToString(), "application/json");
    }
}