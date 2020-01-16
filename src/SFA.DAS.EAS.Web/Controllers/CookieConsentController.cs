using System.Web.Mvc;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers
{
    public class CookieConsentController : Controller
    {
        [HttpGet]        
        [Route("cookieConsent")]
        public ActionResult CookieConsent()
        {
            return Redirect(Url.EmployerAccountsAction("cookieConsent/settings", false));
        }

        [HttpGet]
        [Route("accounts/{HashedAccountId}/cookieConsent")]        
        public ActionResult CookieConsentWithHashedAccountId()
        {
            return Redirect(Url.EmployerAccountsAction("cookieConsent/settings", true));
        }


    }
}