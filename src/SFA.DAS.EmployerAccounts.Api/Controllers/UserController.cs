using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Api.Authorization;
using SFA.DAS.EmployerAccounts.Commands.UpsertRegisteredUser;
using SFA.DAS.EmployerAccounts.Queries.GetUserByEmail;

namespace SFA.DAS.EmployerAccounts.Api.Controllers;

[Authorize(Policy = ApiRoles.ReadUserAccounts)]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserController> _logger;

    public UserController(IMediator mediator, ILogger<UserController> logger)
    {
        _mediator = mediator;
        _logger = logger;
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
        catch(Exception e)
        {
            _logger.LogError(e,"Error in UserController PUT");
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }
}