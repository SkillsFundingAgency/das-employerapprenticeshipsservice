using SFA.DAS.EmployerUsers.WebClientComponents;
using System.Web.Mvc;
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
            return Redirect(Url.EmployerAccountsAction("service/accounts"));
        }

        [AuthoriseActiveUser]
        [HttpGet]
        [Route("accounts")]
        public ActionResult ViewAccounts()
        {
            return Redirect(Url.EmployerAccountsAction("service/accounts", false));
        }

        [HttpGet]
        [Route("usedServiceBefore")]
        public ActionResult UsedServiceBefore()
        {
            return Redirect(Url.EmployerAccountsAction("service/usedServiceBefore", false));
        }

        [HttpGet]
        [Route("whatYoullNeed")]
        public ActionResult WhatYoullNeed()
        {
            return Redirect(Url.EmployerAccountsAction("service/whatYoullNeed", false));
        }

        [HttpGet]
        [Route("register")]
        public ActionResult RegisterUser()
        {
            return Redirect(Url.EmployerAccountsAction("service/register", false));
        }

        [Authorize]
        [HttpGet]
        [Route("register/new")]
        public ActionResult HandleNewRegistration()
        {
            return Redirect(Url.EmployerAccountsAction("service/register/new", false));
        }

        [Authorize]
        [HttpGet]
        [Route("password/change")]
        public ActionResult HandlePasswordChanged(bool userCancelled = false)
        {
            return Redirect(Url.EmployerAccountsAction("service/password/change", false));
        }

        [Authorize]
        [HttpGet]
        [Route("email/change")]
        public ActionResult HandleEmailChanged(bool userCancelled = false)
        {
            return Redirect(Url.EmployerAccountsAction("service/email/change", false));
        }

        [Authorize]
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