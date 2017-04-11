using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Api.Attributes;
using SFA.DAS.EAS.Api.Orchestrators;

namespace SFA.DAS.EAS.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/legalentities")]
    public class AccountLegalEntitiesController : ApiController
    {
        private readonly AccountsOrchestrator _orchestrator;

        public AccountLegalEntitiesController(AccountsOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Route("", Name = "GetLegalEntities")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLegalEntities(string hashedAccountId)
        {
            var result = await _orchestrator.GetAccount(hashedAccountId);

            if (result.Data == null)
            {
                return NotFound();
            }

            result.Data.LegalEntities.ForEach(x => CreateGetLegalEntityLink(hashedAccountId, x));
            return Ok(result.Data.LegalEntities);
        }

        [Route("{legalentityid}", Name = "GetLegalEntity")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLegalEntity(string hashedAccountId, long legalEntityId)
        {
            var result = await _orchestrator.GetLegalEntity(hashedAccountId, legalEntityId);

            if (result.Data == null)
            {
                return NotFound();
            }

            return Ok(result.Data);
        }

        private void CreateGetLegalEntityLink(string hashedAccountId, ResourceViewModel legalEntity)
        {
            legalEntity.Href = Url.Route("GetLegalEntity", new { hashedAccountId, legalEntityId = legalEntity.Id });
        }
    }
}