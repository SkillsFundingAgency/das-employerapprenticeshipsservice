using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerAccounts.Api.Authorization;
using SFA.DAS.EmployerAccounts.Commands.UpsertRegisteredUser;
using SFA.DAS.EmployerAccounts.Queries.GetUserByEmail;

namespace SFA.DAS.EmployerAccounts.Api.Controllers;

[Authorize(Policy = ApiRoles.ReadUserAccounts)]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Get(string email)
    {
        var response = await _mediator.Send(new GetUserByEmailQuery{Email = email});

        if (response.User == null) return NotFound();
        return Ok(response.User);
    }
    
    [HttpPut]
    [Route("upsert")]
    public async Task<IActionResult> Upsert([FromBody] UpsertRegisteredUserCommand command)
    {
        try
        {
            await _mediator.Send(command);
            return Ok();
        }
        catch
        {
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }
}