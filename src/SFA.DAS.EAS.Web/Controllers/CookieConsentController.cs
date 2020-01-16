using System.Web.Mvc;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers
{    
    public class CookieConsentController : Controller
    {
        [HttpGet]
        [Route("accounts/{HashedAccountId}/cookieConsent", Order = 1)]
        [Route("cookieConsent", Order = 2)]
        public ActionResult CookieConsent()
        {
            return Redirect(Url.EmployerAccountsAction("cookieConsent/settings", false));
        }
    }
}