using System.Web;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Web.Extensions;
using SFA.DAS.EmployerFinance.Web.Helpers;
using System.Web.Mvc;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Web.Controllers
{
    [RoutePrefix("service")]
    public class HomeController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IAuthenticationService _owinWrapper;
        private readonly EmployerFinanceConfiguration _configuration;

        public HomeController(IAuthenticationService owinWrapper, EmployerFinanceConfiguration configuration)
        {
            _owinWrapper = owinWrapper;
            _configuration = configuration;
        }

        public Microsoft.AspNetCore.Mvc.ActionResult Index()
        {
            return Redirect(Url.LegacyEasAction(string.Empty));
        }

        [Authorize]
        public Microsoft.AspNetCore.Mvc.ActionResult SignedIn()
        {
            return View();
        }

        [HttpGet]
        [Route("{HashedAccountId}/privacy", Order = 0)]
        [Route("privacy", Order = 1)]
        public Microsoft.AspNetCore.Mvc.ActionResult Privacy()
        {
            return Redirect(Url.EmployerAccountsAction("service", "privacy"));
        }

        [HttpGet]
        [Route("{HashedAccountId}/cookieConsent", Order = 0)]
        [Route("cookieConsent", Order = 1)]
        public Microsoft.AspNetCore.Mvc.ActionResult CookieConsent()
        {
            return Redirect(Url.EmployerAccountsAction("cookieConsent"));
        }       

        [Route("signOut")]
        public Microsoft.AspNetCore.Mvc.ActionResult SignOut()
        {
            _owinWrapper.SignOutUser();

            var owinContext = HttpContext.GetOwinContext();
            var authenticationManager = owinContext.Authentication;
            var idToken = authenticationManager.User.FindFirst("id_token")?.Value;
            var constants = new Constants(_configuration.Identity);

            return new Microsoft.AspNetCore.Mvc.RedirectResult(string.Format(constants.LogoutEndpoint(), idToken));
        }

        [Route("SignOutCleanup")]
        public void SignOutCleanup()
        {
            _owinWrapper.SignOutUser();
        }

        [HttpGet]
        [Route("help")]
        public Microsoft.AspNetCore.Mvc.ActionResult Help()
        {
            return RedirectPermanent(_configuration.ZenDeskHelpCentreUrl);
        }

        [Authorize]
        [Route("signIn")]
        public Microsoft.AspNetCore.Mvc.ActionResult SignIn()
        {
            return RedirectToAction(ControllerConstants.IndexActionName);
        }       
    }
}