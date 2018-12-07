using System.Web.Mvc;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EAS.Web.Controllers
{
    [RoutePrefix("settings")]
    [AuthoriseActiveUser]
    public class SettingsController : Controller
    {
        [HttpGet]
        [Route("notifications")]
        public ActionResult NotificationSettings()
        {
            return Redirect(Url.EmployerAccountsAction("settings/notifications", false));
        }

        [HttpGet]
        [Route("notifications/unsubscribe/{hashedAccountId}")]
        public ActionResult NotificationUnsubscribe(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsAction($"settings/notifications/unsubscribe/{hashedAccountId}", false));
        }

    }
}