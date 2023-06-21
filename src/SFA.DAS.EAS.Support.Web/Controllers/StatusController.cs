using System.Reflection;
using Microsoft.AspNetCore.Http.Extensions;
using SFA.DAS.EAS.Support.ApplicationServices;
using SFA.DAS.EAS.Support.Web.Authorization;

namespace SFA.DAS.EAS.Support.Web.Controllers;

[AllowAnonymous]
[Route("api/status")]
public class StatusController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            ServiceName = "SFA DAS Employer Apprenticeship Service Support Site",
            ServiceVersion = Assembly.GetExecutingAssembly().Version(),
            ServiceTime = DateTimeOffset.UtcNow,
            Request = $" {HttpContext.Request.Method}: {HttpContext.Request.GetDisplayUrl()}"
        });
    }
}