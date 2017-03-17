using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EAS.Api.Orchestrators;

namespace SFA.DAS.EAS.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/legalEntities/{hashedlegalEntityId}/agreements")]
    public class EmployerAgreementController : ApiController
    {
        private readonly AgreementOrchestrator _orchestrator;

        public EmployerAgreementController(AgreementOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Route("{agreementId}", Name = "AgreementById")]
        [Authorize(Roles = "ReadAllEmployerAgreements")]
        [HttpGet]   
        public async Task<IHttpActionResult> GetAgreement(string agreementId)
        {
            var response = await _orchestrator.GetAgreement(agreementId);

            return Ok(response);
        }
    }
}
