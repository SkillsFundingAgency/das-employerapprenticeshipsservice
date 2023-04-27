using Microsoft.Extensions.Configuration;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EAS.Web.Controllers;

[Route("settings")]
[AuthoriseActiveUser]
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