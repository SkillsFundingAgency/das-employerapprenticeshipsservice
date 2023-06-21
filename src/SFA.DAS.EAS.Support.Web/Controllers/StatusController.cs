using System.Reflection;
using Microsoft.AspNetCore.Http.Extensions;
using SFA.DAS.EAS.Support.ApplicationServices;
using SFA.DAS.EAS.Support.Web.Authorization;

namespace SFA.DAS.EAS.Support.Web.Controllers;

[Authorize(Policy = PolicyNames.IsSupportPortalUser)]
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