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
        [Route("settings")]
        [Route("settings/{saved}")]
        public ActionResult Settings(bool saved = false)
        {
            return View(new { Saved = saved });
        }

        [HttpPost]
        [Route("settings")]
        public ActionResult Settings(bool analyticsConsent, bool marketingConsent)
        {
            var cookies = new List<HttpCookie>
            {
                new HttpCookie("CookieConsent", true.ToString()),
                new HttpCookie("AnalyticsConsent", analyticsConsent.ToString()),
                new HttpCookie("MarketingConsent", marketingConsent.ToString())
            };

            cookies.ForEach(x => ControllerContext.HttpContext.Response.Cookies.Add(x));

            return RedirectToAction("Settings", new { saved = true });
        }

        [HttpGet]
        [Route("details")]
        public ActionResult Details()
        {
            return View();
        }
    }
}