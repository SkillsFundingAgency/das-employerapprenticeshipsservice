using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.EmployerAccounts.Api.Controllers;

[Route("ping")]
public class PingController : ControllerBase
{
    [HttpGet, AllowAnonymous]
    public IActionResult Get()
    {
        return Ok();
    }
}