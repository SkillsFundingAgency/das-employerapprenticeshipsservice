using System.Web.Mvc;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers
{
    [RoutePrefix("cookieConsent")]
    public class CookieConsentController : Controller
    {
        [HttpGet]        
        public ActionResult CookieConsent()
        {
            return Redirect(Url.EmployerAccountsAction("cookieConsent/settings", false));
        }
    }
}