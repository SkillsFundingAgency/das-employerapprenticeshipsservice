using SFA.DAS.Authenication;
using SFA.DAS.EmployerAccounts.Configuration;
using System.Web;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthenticationService _owinWrapper;
        private readonly EmployerAccountsConfiguration _configuration;

        public HomeController(IAuthenticationService owinWrapper, EmployerAccountsConfiguration configuration)
        {
            _owinWrapper = owinWrapper;
            _configuration = configuration;
        }
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult SignedIn()
        {
            return View();
        }

        [Route("signOut")]
        public ActionResult SignOut()
        {
            _owinWrapper.SignOutUser();

            var owinContext = HttpContext.GetOwinContext();
            var authenticationManager = owinContext.Authentication;
            var idToken = authenticationManager.User.FindFirst("id_token")?.Value;
            var constants = new Constants(_configuration.Identity);

            return new RedirectResult(string.Format(constants.LogoutEndpoint(), idToken, owinContext.Request.Uri.Scheme, owinContext.Request.Uri.Authority));
        }
    }
}