using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
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

    [Route("{payeschemeref}", Name = "GetPayeScheme")]
    [DasAuthorize(Roles = "ReadAllEmployerAccountBalances")]
    [HttpGet]
    public async Task<IActionResult> GetPayeScheme(string hashedAccountId, string payeSchemeRef)
    {
        var result = await _orchestrator.GetPayeScheme(hashedAccountId, WebUtility.UrlDecode(payeSchemeRef));

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [Route("", Name = "GetPayeSchemes")]
    [DasAuthorize(Roles = "ReadAllEmployerAccountBalances")]
    [HttpGet]
    public async Task<IActionResult> GetPayeSchemes(string hashedAccountId)
    {
        var result = await _orchestrator.GetPayeSchemesForAccount(hashedAccountId);


        if (result == null)
        {
            return NotFound();
        }

        return Ok(
            new ResourceList(
                result
                    .Select(
                        pv => new Resource
                        {
                            Id = pv.Ref,
                            Href = Url.RouteUrl(
                                "GetPayeScheme",
                                new
                                {
                                    hashedAccountId,
                                    payeSchemeRef = WebUtility.UrlEncode(pv.Ref)
                                })
                        }))
        );
    }
}