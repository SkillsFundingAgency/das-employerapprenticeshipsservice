using SFA.DAS.EmployerUsers.WebClientComponents;
using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers
{
    [RoutePrefix("service")]
    public class HomeController : Controller
    {
        [Route("~/")]
        [Route]
        [Route("Index")]
        public ActionResult Index()
        {
            return Redirect(Url.EmployerAccountsAction("service/index", false));
        }

        [AuthoriseActiveUser]
        [HttpGet]
        [Route("accounts")]
        public ActionResult ViewAccounts()
        {
            return Redirect(Url.EmployerAccountsAction("service/accounts", false));
        }

        [HttpGet]
        [Route("register")]
        public ActionResult RegisterUser()
        {
            return Redirect(Url.EmployerAccountsAction("service/register", false));
        }

        [DasAuthorize]
        [HttpGet]
        [Route("register/new")]
        public ActionResult HandleNewRegistration()
        {
            return Redirect(Url.EmployerAccountsAction("service/register/new", false));
        }

        [DasAuthorize]
        [HttpGet]
        [Route("password/change")]
        public ActionResult HandlePasswordChanged(bool userCancelled = false)
        {
            return Redirect(Url.EmployerAccountsAction("service/password/change", false));
        }

        [DasAuthorize]
        [HttpGet]
        [Route("email/change")]
        public ActionResult HandleEmailChanged(bool userCancelled = false)
        {
            return Redirect(Url.EmployerAccountsAction("service/email/change", false));
        }

        [DasAuthorize]
        [Route("signIn")]
        public ActionResult SignIn()
        {
            return Redirect(Url.EmployerAccountsAction("service/signIn", false));
        }

        [Route("signOut")]
        public ActionResult SignOut()
        {
            return Redirect(Url.EmployerAccountsAction("service/signOut", false));
        }

        [HttpGet]
        [Route("privacy")]
        public ActionResult Privacy()
        {
            return Redirect(Url.EmployerAccountsAction("service/privacy", false));
        }

        [HttpGet]
        [Route("help")]
        public ActionResult Help()
        {
            return Redirect(Url.EmployerAccountsAction("service/help", false));
        }

        [HttpGet]
        [Route("start")]
        public ActionResult ServiceStartPage()
        {
            return Redirect(Url.EmployerAccountsAction("service/start", false));
        }
    }
}