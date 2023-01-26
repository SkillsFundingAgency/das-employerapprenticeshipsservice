using SFA.DAS.EmployerUsers.WebClientComponents;
using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers
{
    [RoutePrefix("service")]
    public class HomeController : Microsoft.AspNetCore.Mvc.Controller
    {
        private const string GoogleTag = "_ga";

        [Route("~/")]
        [Route]
        [Route("Index")]
        public Microsoft.AspNetCore.Mvc.ActionResult Index()
        {
            return Redirect(Url.EmployerAccountsAction($"service/index?{GetTrackerQueryString()}", false));
        }        

        [AuthoriseActiveUser]
        [HttpGet]
        [Route("accounts")]
        public Microsoft.AspNetCore.Mvc.ActionResult ViewAccounts()
        {
            return Redirect(Url.EmployerAccountsAction("service/accounts", false));
        }

        [HttpGet]
        [Route("register")]
        public Microsoft.AspNetCore.Mvc.ActionResult RegisterUser()
        {
            return Redirect(Url.EmployerAccountsAction("service/register", false));
        }

        [DasAuthorize]
        [HttpGet]
        [Route("register/new")]
        public Microsoft.AspNetCore.Mvc.ActionResult HandleNewRegistration()
        {
            return Redirect(Url.EmployerAccountsAction("service/register/new", false));
        }

        [DasAuthorize]
        [HttpGet]
        [Route("password/change")]
        public Microsoft.AspNetCore.Mvc.ActionResult HandlePasswordChanged(bool userCancelled = false)
        {
            return Redirect(Url.EmployerAccountsAction("service/password/change", false));
        }

        [DasAuthorize]
        [HttpGet]
        [Route("email/change")]
        public Microsoft.AspNetCore.Mvc.ActionResult HandleEmailChanged(bool userCancelled = false)
        {
            return Redirect(Url.EmployerAccountsAction("service/email/change", false));
        }

        [DasAuthorize]
        [Route("signIn")]
        public Microsoft.AspNetCore.Mvc.ActionResult SignIn()
        {
            return Redirect(Url.EmployerAccountsAction("service/signIn", false));
        }

        [Route("signOut")]
        public Microsoft.AspNetCore.Mvc.ActionResult SignOut()
        {
            return Redirect(Url.EmployerAccountsAction("service/signOut", false));
        }

        [HttpGet]
        [Route("{HashedAccountId}/privacy", Order = 0)]
        [Route("privacy", Order = 1)]
        public Microsoft.AspNetCore.Mvc.ActionResult Privacy()
        {
            return Redirect(Url.EmployerAccountsAction("service/privacy", false));
        }

        [HttpGet]
        [Route("cookieConsent")]
        public Microsoft.AspNetCore.Mvc.ActionResult CookieConsent()
        {
            return Redirect(Url.EmployerAccountsAction("cookieConsent/settings", false));
        }

        [HttpGet]
        [Route("help")]
        public Microsoft.AspNetCore.Mvc.ActionResult Help()
        {
            return Redirect(Url.EmployerAccountsAction("service/help", false));
        }

        [HttpGet]
        [Route("start")]
        public Microsoft.AspNetCore.Mvc.ActionResult ServiceStartPage()
        {
            return Redirect(Url.EmployerAccountsAction("service/start", false));
        }

        [HttpGet]
        [Route("termsAndConditions/overview")]
        public Microsoft.AspNetCore.Mvc.ActionResult TermsAndConditionsOverview()
        {
            return Redirect(Url.EmployerAccountsAction("service/termsAndConditions/overview", false));
        }

        private string GetTrackerQueryString()
        {
            var trackerValue = Url.RequestContext.HttpContext.Request.QueryString[GoogleTag];
            return trackerValue == null ? string.Empty : $"{GoogleTag}={trackerValue}";
        }
    }
}