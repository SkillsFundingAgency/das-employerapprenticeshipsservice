using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EmployerAccounts.Api.Attributes;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;

namespace SFA.DAS.EmployerAccounts.Api.Controllers
{
    [RoutePrefix("api/accounts")]
    public class EmployerAgreementController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private readonly AgreementOrchestrator _orchestrator;
        
        public EmployerAgreementController(AgreementOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Route("{hashedAccountId}/legalEntities/{hashedlegalEntityId}/agreements/{agreementId}", Name = "AgreementById")]
        [ApiAuthorize(Roles = "ReadAllEmployerAgreements")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAgreement(string agreementId)
        {
            var response = await _orchestrator.GetAgreement(agreementId);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [Route("internal/{accountId}/minimum-signed-agreement-version", Name = "GetMinimumSignedAgreemmentVersion")]
        [ApiAuthorize(Roles = "ReadAllEmployerAgreements")]
        [HttpGet]
        public async Task<IHttpActionResult> GetMinimumSignedAgreemmentVersion(long accountId)
        {
            var result = await _orchestrator.GetMinimumSignedAgreemmentVersion(accountId);
            return Ok(result);
        }
    }
}
