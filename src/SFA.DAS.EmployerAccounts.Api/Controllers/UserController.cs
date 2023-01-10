using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using SFA.DAS.EmployerAccounts.Queries.GetUserByEmail;

namespace SFA.DAS.EmployerAccounts.Api.Controllers
{
    [Authorize(Roles = "ReadUserAccounts")]
    [RoutePrefix("api/user")]
    public class UserController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("")]
        public async Task<IHttpActionResult> Get(string email)
        {
            var response = await _mediator.SendAsync(new GetUserByEmailQuery{Email = email});

            if (response.User == null) return NotFound();
            return Ok(response.User);
        }
    }
}
