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
        public Microsoft.AspNetCore.Mvc.ActionResult Settings()
        {
            return View();
        }

        [HttpGet]
        [Route("accounts/{HashedAccountId}/cookieConsent/details", Order = 0)]
        [Route("cookieConsent/details", Order = 1)]
        public Microsoft.AspNetCore.Mvc.ActionResult Details()
        {
            return View();
        }
    }
}