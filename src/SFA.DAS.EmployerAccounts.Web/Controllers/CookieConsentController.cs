using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [RoutePrefix("cookieConsent")]
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
        [Route("{HashedAccountId}/settings/{saved}", Order = 0)]
        [Route("{HashedAccountId}/settings", Order = 1)]
        [Route("settings/{saved}", Order = 2)]
        [Route("settings", Order = 3)]
        public ActionResult Settings(bool saved = false)
        {
            return View(new { Saved = saved });
        }

        [HttpPost]
        [Route("{HashedAccountId}/settings", Order = 0)]
        [Route("settings", Order = 1)]
        public ActionResult Settings(bool analyticsConsent, bool marketingConsent)
        {
            var cookies = new List<HttpCookie>
            {
                new HttpCookie("CookieConsent", true.ToString().ToLower()),
                new HttpCookie("AnalyticsConsent", analyticsConsent.ToString().ToLower()),
                new HttpCookie("MarketingConsent", marketingConsent.ToString().ToLower())
            };

            cookies.ForEach(x => ControllerContext.HttpContext.Response.Cookies.Add(x));

            return RedirectToAction("Settings", new { saved = true });
        }

        [HttpGet]
        [Route("{HashedAccountId}/details", Order = 0)]
        [Route("details", Order = 1)]
        public ActionResult Details()
        {
            return View();
        }
    }
}