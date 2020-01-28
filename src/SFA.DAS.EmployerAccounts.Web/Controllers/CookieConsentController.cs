using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{    
    public class CookieConsentController : BaseController
    {
        public CookieConsentController(
            IAuthenticationService owinWrapper,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage
        ) : base(owinWrapper, multiVariantTestingService, flashMessage)
        {

        }

        [HttpGet]
        [Route("accounts/{HashedAccountId}/cookieConsent/{saved}", Order = 0)]
        [Route("accounts/{HashedAccountId}/cookieConsent", Order = 1)]
        [Route("cookieConsent/{saved}", Order = 2)]
        [Route("cookieConsent", Order = 3)]        
        public ActionResult Settings(bool saved = false)
        {
            return View(new { Saved = saved });
        }

        [HttpPost]
        [Route("accounts/{HashedAccountId}/cookieConsent", Order = 0)]
        [Route("cookieConsent", Order = 1)]        
        public ActionResult Settings(bool analyticsConsent, bool marketingConsent)
        {
            var cookies = new List<HttpCookie>
            {
                new HttpCookie("DASSeenCookieMessage", true.ToString().ToLower()),
                new HttpCookie("AnalyticsConsent", analyticsConsent.ToString().ToLower()),
                new HttpCookie("MarketingConsent", marketingConsent.ToString().ToLower())
            };

            cookies.ForEach(x => ControllerContext.HttpContext.Response.Cookies.Add(x));

            return RedirectToAction("Settings", new { saved = true });
        }

        [HttpGet]
        [Route("accounts/{HashedAccountId}/cookieConsent/details", Order = 0)]
        [Route("cookieConsent/details", Order = 1)]
        public ActionResult Details()
        {
            return View();
        }
    }
}