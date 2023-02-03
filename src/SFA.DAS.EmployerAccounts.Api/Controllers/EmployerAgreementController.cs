using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerAccounts.Api.Authorization;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;

namespace SFA.DAS.EmployerAccounts.Api.Controllers;

[Route("api/accounts")]
public class EmployerAgreementController : ControllerBase
{
    private readonly AgreementOrchestrator _orchestrator;
        
    public EmployerAgreementController(AgreementOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    [Route("{hashedAccountId}/legalEntities/{hashedlegalEntityId}/agreements/{agreementId}", Name = "AgreementById")]
    [DasAuthorize(Roles = ApiRoles.ReadAllEmployerAgreements)]
    [HttpGet]
    public async Task<IActionResult> GetAgreement(string agreementId)
    {
        var response = await _orchestrator.GetAgreement(agreementId);

        if (response == null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    [Route("internal/{accountId}/minimum-signed-agreement-version", Name = "GetMinimumSignedAgreemmentVersion")]
    [DasAuthorize(Roles = ApiRoles.ReadAllEmployerAgreements)]
    [HttpGet]
    public async Task<IActionResult> GetMinimumSignedAgreemmentVersion(long accountId)
    {
        var result = await _orchestrator.GetMinimumSignedAgreemmentVersion(accountId);
        return Ok(result);
    }
}