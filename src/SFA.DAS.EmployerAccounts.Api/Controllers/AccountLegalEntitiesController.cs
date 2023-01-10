using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using SFA.DAS.EmployerAccounts.Api.Attributes;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntities.Api;

namespace SFA.DAS.EmployerAccounts.Api.Controllers
{
    [ApiAuthorize(Roles = "ReadUserAccounts")]
    [RoutePrefix("api/accountlegalentities")]
    public class AccountLegalEntitiesController : Microsoft.AspNetCore.Mvc.ControllerBase
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
