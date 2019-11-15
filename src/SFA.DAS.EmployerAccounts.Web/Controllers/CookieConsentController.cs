using System;
using System.Collections.Generic;
using System.Linq;
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
        public ActionResult Settings()
        {
            return View();
        }

        [HttpPost]
        [Route("settings")]
        public ActionResult Settings()
        {
            return View();
        }
    }
}