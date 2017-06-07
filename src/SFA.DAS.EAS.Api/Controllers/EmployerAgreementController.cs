using System.Threading.Tasks;
using System.Web.Http;
using NLog;
using SFA.DAS.EAS.Api.Attributes;
using SFA.DAS.EAS.Api.Orchestrators;

namespace SFA.DAS.EAS.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/legalEntities/{hashedlegalEntityId}/agreements")]
    public class EmployerAgreementController : ApiController
    {
        private readonly AgreementOrchestrator _orchestrator;
        private readonly ILogger _logger;

        public EmployerAgreementController(AgreementOrchestrator orchestrator, ILogger logger)
        {
            _orchestrator = orchestrator;
            _logger = logger;
        }

        [Route("{agreementId}", Name = "AgreementById")]
        [ApiAuthorize(Roles = "ReadAllEmployerAgreements")]
        [HttpGet]   
        public async Task<IHttpActionResult> GetAgreement(string agreementId)
        {
            var response = await _orchestrator.GetAgreement(agreementId);
            
            _logger.Info("Incorrect get agreement API call received");

            return Ok(response);
        }

        [Route("{agreementId}/agreement", Name = "AgreementById")]
        [ApiAuthorize(Roles = "ReadAllEmployerAgreements")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAgreementById(string agreementId)
        {
            var response = await _orchestrator.GetAgreement(agreementId);

            if (response.Data == null)
            {
                return NotFound();
            }

            return Ok(response.Data);
        }

    }
}
