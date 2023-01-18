using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntities.Api;

namespace SFA.DAS.EmployerAccounts.Api.Controllers
{
    [Authorize(Roles = "ReadUserAccounts")]
    [Route("api/accountlegalentities")]
    public class AccountLegalEntitiesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountLegalEntitiesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Get([FromQuery] GetAccountLegalEntitiesQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response.AccountLegalEntities);
        }
    }
}
