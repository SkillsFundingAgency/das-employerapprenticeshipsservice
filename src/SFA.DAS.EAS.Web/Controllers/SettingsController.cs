using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers;

[Route("settings")]
[Authorize(Policy = nameof(PolicyNames.HasUserAccount))]
public class SettingsController : Controller
{
    public IConfiguration Configuration { get; set; }
    public SettingsController(IConfiguration _configuration)
    {
        Configuration = _configuration;
    }
    [HttpGet]
    [Route("notifications")]
    public IActionResult NotificationSettings()
    {
        return Redirect(Url.EmployerAccountsAction("settings/notifications", Configuration, false));
    }

    [HttpGet]
    [Route("notifications/unsubscribe/{hashedAccountId}")]
    public IActionResult NotificationUnsubscribe(string hashedAccountId)
    {
        return Redirect(Url.EmployerAccountsAction($"settings/notifications/unsubscribe/{hashedAccountId}", Configuration, false));
    }

}