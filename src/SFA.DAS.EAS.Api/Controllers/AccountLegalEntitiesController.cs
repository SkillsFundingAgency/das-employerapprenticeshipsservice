using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities.Api;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [ApiAuthorize(Roles = "ReadUserAccounts")]
    [RoutePrefix("api/accountlegalentities")]
    public class AccountLegalEntitiesController : ApiController
    {
        private readonly IMediator _mediator;

        public AccountLegalEntitiesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route]
        public async Task<IHttpActionResult> Get([FromUri] GetAccountLegalEntitiesQuery query)
        {
            var response = await _mediator.SendAsync(query);
            return Ok(response.AccountLegalEntities);
        }
    }
}
