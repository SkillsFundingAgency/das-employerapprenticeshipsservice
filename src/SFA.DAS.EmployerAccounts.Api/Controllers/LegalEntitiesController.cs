using System;
using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using SFA.DAS.EmployerAccounts.Api.Attributes;
using SFA.DAS.Validation.WebApi;

namespace SFA.DAS.EmployerAccounts.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/legalentities")]
    public class LegalEntitiesController : ApiController
    {
        private readonly IMediator _mediator;

        public LegalEntitiesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("", Name = "GetLegalEntities")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLegalEntities(string hashedAccountId)
        {
            throw new NotImplementedException();
            //var result = await _orchestrator.GetAccount(hashedAccountId);

//            if (result.Data == null)
//            {
//                return NotFound();
//            }
//            
//            result.Data.LegalEntities.ForEach(l => l.Href = Url.Route("GetLegalEntity", new { hashedAccountId, legalEntityId = l.Id }));
//
//            return Ok(result.Data.LegalEntities);
        }
    }
}