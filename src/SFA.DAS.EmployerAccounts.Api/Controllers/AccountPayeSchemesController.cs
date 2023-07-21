using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Api.Authorization;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EmployerAccounts.Api.Controllers;

[Route("api/accounts/{hashedAccountId}/payeschemes")]
public class AccountPayeSchemesController : ControllerBase
{
    private readonly AccountsOrchestrator _orchestrator;
    private readonly ILogger<AccountPayeSchemesController> _logger;

    public AccountPayeSchemesController(AccountsOrchestrator orchestrator, ILogger<AccountPayeSchemesController> logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;
    }

    [Route("", Name = "GetPayeSchemes")]
    [Authorize(Policy = ApiRoles.ReadAllEmployerAccountBalances)]
    [HttpGet]
    public async Task<IActionResult> GetPayeSchemes([FromRoute] string hashedAccountId)
    {
        var result = await _orchestrator.GetPayeSchemesForAccount(hashedAccountId);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(new ResourceList(result.Select(pv => new Resource
        {
            Id = pv.Ref,
            Href = Url.RouteUrl("GetPayeScheme", new { hashedAccountId, payeSchemeRef = Uri.EscapeDataString(pv.Ref) })
        })));
    }

    [Route("scheme", Name = "GetPayeScheme")]
    [Authorize(Policy = ApiRoles.ReadAllEmployerAccountBalances)]
    [HttpGet]
    public async Task<IActionResult> GetPayeScheme([FromRoute] string hashedAccountId, [FromQuery] string payeSchemeRef)
    {
        var decodedPayeSchemeRef = Uri.UnescapeDataString(payeSchemeRef);

        var result = await _orchestrator.GetPayeScheme(hashedAccountId, decodedPayeSchemeRef);

        if (result == null)
        {
            _logger.LogDebug("The PAYE scheme {PayeScheme} was not found.", decodedPayeSchemeRef);
            return NotFound();
        }

        return Ok(result);
    }
}