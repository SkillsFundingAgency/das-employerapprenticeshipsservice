using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerAccounts.Api.Authorization;
using SFA.DAS.EmployerAccounts.Queries.GetUserByEmail;

namespace SFA.DAS.EmployerAccounts.Api.Controllers;

[DasAuthorize(Roles = ApiRoles.ReadUserAccounts)]
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
}