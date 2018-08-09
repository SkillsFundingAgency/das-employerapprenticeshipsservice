using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerUsers.WebClientComponents;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [RoutePrefix("settings")]
    [AuthoriseActiveUser]
    public class SettingsController : Controller
    {
        [HttpGet]
        [Route("notifications")]
        public ActionResult NotificationSettings()
        {
            return Redirect(Url.LegacyEasAction("settings/notifications"));
        }

        [HttpGet]
        [Route("notifications/unsubscribe/{hashedAccountId}")]
        public ActionResult NotificationUnsubscribe(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAction($"settings/notifications/unsubscribe/{hashedAccountId}"));
        }
    }
}