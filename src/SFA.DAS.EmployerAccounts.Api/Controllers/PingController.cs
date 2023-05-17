using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.EmployerAccounts.Api.Controllers;

[AllowAnonymous]
[Route("ping")]
public class PingController : ControllerBase
{
    [HttpGet]
    [Route("")]
    public IActionResult Get()
    {
        return Ok();
    }
}