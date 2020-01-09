using System.Web;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Web.Extensions;
using SFA.DAS.EmployerFinance.Web.Helpers;
using System.Web.Mvc;

namespace SFA.DAS.EmployerFinance.Web.Controllers
{
    [RoutePrefix("service")]
    public class HomeController : Controller
    {
        private readonly IAuthenticationService _owinWrapper;
        private readonly EmployerFinanceConfiguration _configuration;

        public HomeController(IAuthenticationService owinWrapper, EmployerFinanceConfiguration configuration)
        {
            _owinWrapper = owinWrapper;
            _configuration = configuration;
        }

        public ActionResult Index()
        {
            return Redirect(Url.LegacyEasAction(string.Empty));
        }

        [Authorize]
        public ActionResult SignedIn()
        {
            return View();
        }

        [HttpGet]
        [Route("{HashedAccountId}/privacy", Order = 0)]
        [Route("privacy", Order = 1)]
        public ActionResult Privacy()
        {
            return Redirect(Url.EmployerAccountsAction("service", "privacy"));
        }

        [HttpGet]
        [Route("{HashedAccountId}/cookieConsent", Order = 0)]
        [Route("cookieConsent", Order = 1)]
        public ActionResult CookieConsent()
        {
            return Redirect(Url.EmployerAccountsAction("cookieConsent", "settings"));
        }

        [Route("signOut")]
        public ActionResult SignOut()
        {
            _owinWrapper.SignOutUser();

            var owinContext = HttpContext.GetOwinContext();
            var authenticationManager = owinContext.Authentication;
            var idToken = authenticationManager.User.FindFirst("id_token")?.Value;
            var constants = new Constants(_configuration.Identity);

            return new RedirectResult(string.Format(constants.LogoutEndpoint(), idToken));
        }

        [Route("SignOutCleanup")]
        public void SignOutCleanup()
        {
            _owinWrapper.SignOutUser();
        }

        [HttpGet]
        [Route("help")]
        public ActionResult Help()
        {
            return RedirectPermanent(_configuration.ZenDeskHelpCentreUrl);
        }

        [Authorize]
        [Route("signIn")]
        public ActionResult SignIn()
        {
            return RedirectToAction(ControllerConstants.IndexActionName);
        }
    }
}