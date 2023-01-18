using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerAccounts.Queries.GetUserByEmail;

namespace SFA.DAS.EmployerAccounts.Api.Controllers
{
    [Authorize(Roles = "ReadUserAccounts")]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("")]
        public async Task<IActionResult> Get(string email)
        {
            var response = await _mediator.Send(new GetUserByEmailQuery{Email = email});

            if (response.User == null) return NotFound();
            return Ok(response.User);
        }
    }
}
