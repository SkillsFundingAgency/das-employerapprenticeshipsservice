using System.Reflection;
using Microsoft.AspNetCore.Http.Extensions;
using SFA.DAS.Support.Shared;

namespace SFA.DAS.EAS.Support.Web.Controllers;

[Authorize(Roles = "das-support-portal")]
[Route("api/status")]
[ApiController]
public class StatusController : ControllerBase
{
    [AllowAnonymous]
    public IActionResult Get()
    {
        return Ok(new
        {
            ServiceName = "SFA DAS Employer Apprenticeship Service Support Site",
            ServiceVersion = AddServiceVersion(),
            ServiceTime = DateTimeOffset.UtcNow,
            Request = AddRequestContext()
        });
    }

    private static string AddServiceVersion()
    {
        try
        {
            return Assembly.GetExecutingAssembly().Version();
        }
        catch (Exception)
        {
            return "Unknown";
        }
    }

    private string AddRequestContext()
    {
        try
        {
            return $" {HttpContext.Request.Method}: {HttpContext.Request.GetDisplayUrl()}";
        }
        catch
        {
            return "Unknown";
        }
    }
}