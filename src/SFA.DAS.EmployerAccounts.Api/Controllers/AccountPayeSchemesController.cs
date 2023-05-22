using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerAccounts.Api.Authorization;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EmployerAccounts.Api.Controllers;

[Route("api/accounts/{hashedAccountId}/payeschemes")]
public class AccountPayeSchemesController : ControllerBase
{
    private readonly AccountsOrchestrator _orchestrator;

    public AccountPayeSchemesController(AccountsOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    [Route("{payeSchemeRef}", Name = "GetPayeScheme")]
    [Authorize(Policy = ApiRoles.ReadAllEmployerAccountBalances)]
    [HttpGet]
    public async Task<IActionResult> GetPayeScheme([FromRoute]string hashedAccountId, [FromRoute]string payeSchemeRef)
    {
        var decodedPayeSchemeRef = Uri.UnescapeDataString(payeSchemeRef);
        var result = await _orchestrator.GetPayeScheme(hashedAccountId, decodedPayeSchemeRef);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [Route("", Name = "GetPayeSchemes")]
    [Authorize(Policy = ApiRoles.ReadAllEmployerAccountBalances)]
    [HttpGet]
    public async Task<IActionResult> GetPayeSchemes([FromRoute]string hashedAccountId)
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
}