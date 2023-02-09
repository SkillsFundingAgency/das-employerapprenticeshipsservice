using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerAccounts.Api.Authorization;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Api.Controllers;

[Route("api/accounts/{hashedAccountId}/payeschemes")]
public class AccountPayeSchemesController : ControllerBase
{
    private readonly AccountsOrchestrator _orchestrator;
    private readonly IEncodingService _encodingService;

    public AccountPayeSchemesController(AccountsOrchestrator orchestrator, IEncodingService encodingService)
    {
        _orchestrator = orchestrator;
        _encodingService = encodingService;
    }

    [Route("{payeschemeref}", Name = "GetPayeScheme")]
    [Authorize(Policy = ApiRoles.ReadAllEmployerAccountBalances)]
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
    [Authorize(Policy = ApiRoles.ReadAllEmployerAccountBalances)]
    [HttpGet]
    public async Task<IActionResult> GetPayeSchemes(string hashedAccountId)
    {
        var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);
        var result = await _orchestrator.GetPayeSchemesForAccount(accountId);


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