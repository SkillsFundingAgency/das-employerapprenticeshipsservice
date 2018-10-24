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
        [Route("privacy")]
        public ActionResult Privacy()
        {
            return Redirect(Url.LegacyEasAction("service/privacy"));
        }

        [Route("signOut")]
        public ActionResult SignOut()
        {
            _owinWrapper.SignOutUser();

            return new RedirectResult(Url.LegacyEasAction("service/signout"));
        }

        [HttpGet]
        [Route("help")]
        public ActionResult Help()
        {
            return View();
        }

        [Authorize]
        [Route("signIn")]
        public ActionResult SignIn()
        {
            return RedirectToAction(ControllerConstants.IndexActionName);
        }
    }
}