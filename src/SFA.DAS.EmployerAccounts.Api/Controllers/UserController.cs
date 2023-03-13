using MediatR;
using SFA.DAS.EmployerAccounts.Api.Attributes;
using SFA.DAS.EmployerAccounts.Commands.UpsertRegisteredUser;
using SFA.DAS.EmployerAccounts.Queries.GetUserByEmail;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SFA.DAS.EmployerAccounts.Api.Controllers
{
    [ApiAuthorize(Roles = "ReadUserAccounts")]
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("")]
        public async Task<IHttpActionResult> Get(string email)
        {
            var response = await _mediator.SendAsync(new GetUserByEmailQuery { Email = email });

            if (response.User == null) return NotFound();
            return Ok(response.User);
        }

        [HttpPut]
        [Route("upsert")]
        public async Task<IHttpActionResult> Upsert([FromBody] UpsertRegisteredUserCommand command)
        {
            try
            {
                await _mediator.SendAsync(command);
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}
