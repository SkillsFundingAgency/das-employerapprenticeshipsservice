using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Domain.Configuration;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.EAS.Web.Controllers
{
    public class CookieConsentController : Controller
    {
        public IConfiguration Configuration { get; set; }
        public CookieConsentController(IConfiguration _configuration) 
        {
            Configuration = _configuration;
        }
        [HttpGet]
        [Route("cookieConsent", Order = 0)]
        [Route("cookieConsent/settings", Order = 1)]
        public ActionResult CookieConsent()
        {
            return Redirect(Url.EmployerAccountsAction("cookieConsent/settings", Configuration, false));
        }

        [HttpGet]
        [Route("accounts/{HashedAccountId}/cookieConsent", Order = 0)]
        [Route("accounts/{HashedAccountId}/cookieConsent/settings", Order = 1)]
        public ActionResult CookieConsentWithHashedAccountId()
        {
            return Redirect(Url.EmployerAccountsAction("cookieConsent/settings", Configuration, true));
        }


    }
}