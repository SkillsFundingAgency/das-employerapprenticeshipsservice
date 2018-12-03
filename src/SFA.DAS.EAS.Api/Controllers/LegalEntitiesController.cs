using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Application.Queries.GetLegalEntity;
using SFA.DAS.Validation.WebApi;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/legalentities")]
    public class LegalEntitiesController : ApiController
    {
        private readonly AccountsOrchestrator _orchestrator;
        private readonly IMediator _mediator;

        public LegalEntitiesController(AccountsOrchestrator orchestrator, IMediator mediator)
        {
            _orchestrator = orchestrator;
            _mediator = mediator;
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
            
            result.Data.LegalEntities.ForEach(l => l.Href = Url.Route("GetLegalEntity", new { hashedAccountId, legalEntityId = l.Id }));

            return Ok(result.Data.LegalEntities);
        }

        [Route("{legalEntityId}", Name = "GetLegalEntity")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpNotFoundForNullModel]
        public async Task<IHttpActionResult> GetLegalEntity([FromUri] GetLegalEntityQuery query)
        {
            var response = await _mediator.SendAsync(query);
            return Ok(response.LegalEntity);
        }
    }
}