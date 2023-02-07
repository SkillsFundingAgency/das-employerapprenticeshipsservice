using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers
{
    public class CookieConsentController : Controller
    {
        [HttpGet]        
        [Route("cookieConsent", Order = 0)]
        [Route("cookieConsent/settings", Order = 1)]
        public ActionResult CookieConsent()
        {
            return Redirect(Url.EmployerAccountsAction("cookieConsent/settings", false));
        }

        [HttpGet]
        [Route("accounts/{HashedAccountId}/cookieConsent", Order = 0)]
        [Route("accounts/{HashedAccountId}/cookieConsent/settings", Order = 1)]
        public ActionResult CookieConsentWithHashedAccountId()
        {
            return Redirect(Url.EmployerAccountsAction("cookieConsent/settings", true));
        }


    }
}