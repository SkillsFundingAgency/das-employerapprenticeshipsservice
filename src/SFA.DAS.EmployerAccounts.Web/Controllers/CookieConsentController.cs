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
        [Route("accounts/{HashedAccountId}/cookieConsent", Order = 0)]
        [Route("cookieConsent", Order = 1)]        
        public ActionResult Settings(bool saved = false)
        {
            return View(new { Saved = saved });
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