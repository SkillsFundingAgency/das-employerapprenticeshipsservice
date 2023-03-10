using SFA.DAS.EmployerUsers.WebClientComponents;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Route("service")]
    public class HomeController : Controller
    {
        private const string GoogleTag = "_ga";
        public EmployerApprenticeshipsServiceConfiguration Configuration { get; set; }
        public HomeController(EmployerApprenticeshipsServiceConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        [Route("~/")]
        [Route("")]
        [Route("Index")]
        public ActionResult Index()
        {
            return Redirect(Url.EmployerAccountsAction($"service/index?{GetTrackerQueryString()}", Configuration, false));
        }        

        [AuthoriseActiveUser]
        [HttpGet]
        [Route("accounts")]
        public ActionResult ViewAccounts()
        {
            return Redirect(Url.EmployerAccountsAction("service/accounts", Configuration, false));
        }

        [HttpGet]
        [Route("register")]
        public ActionResult RegisterUser()
        {
            return Redirect(Url.EmployerAccountsAction("service/register", Configuration, false));
        }

        [DasAuthorize]
        [HttpGet]
        [Route("register/new")]
        public ActionResult HandleNewRegistration()
        {
            return Redirect(Url.EmployerAccountsAction("service/register/new", Configuration, false));
        }

        [DasAuthorize]
        [HttpGet]
        [Route("password/change")]
        public ActionResult HandlePasswordChanged(bool userCancelled = false)
        {
            return Redirect(Url.EmployerAccountsAction("service/password/change", Configuration, false));
        }

        [DasAuthorize]
        [HttpGet]
        [Route("email/change")]
        public ActionResult HandleEmailChanged(bool userCancelled = false)
        {
            return Redirect(Url.EmployerAccountsAction("service/email/change", Configuration, false));
        }

        [DasAuthorize]
        [Route("signIn")]
        public ActionResult SignIn()
        {
            return Redirect(Url.EmployerAccountsAction("service/signIn", Configuration, false));
        }

        [Route("signOut")]
        public ActionResult SignOut()
        {
            return Redirect(Url.EmployerAccountsAction("service/signOut", Configuration, false));
        }

        [HttpGet]
        [Route("{HashedAccountId}/privacy", Order = 0)]
        [Route("privacy", Order = 1)]
        public ActionResult Privacy()
        {
            return Redirect(Url.EmployerAccountsAction("service/privacy", Configuration, false));
        }

        [HttpGet]
        [Route("cookieConsent")]
        public ActionResult CookieConsent()
        {
            return Redirect(Url.EmployerAccountsAction("cookieConsent/settings", Configuration, false));
        }

        [HttpGet]
        [Route("help")]
        public ActionResult Help()
        {
            return Redirect(Url.EmployerAccountsAction("service/help", Configuration, false));
        }

        [HttpGet]
        [Route("start")]
        public ActionResult ServiceStartPage()
        {
            return Redirect(Url.EmployerAccountsAction("service/start", Configuration, false));
        }

        [HttpGet]
        [Route("termsAndConditions/overview")]
        public ActionResult TermsAndConditionsOverview()
        {
            return Redirect(Url.EmployerAccountsAction("service/termsAndConditions/overview", Configuration, false));
        }

        private string GetTrackerQueryString()
        {
            Url.ActionContext.HttpContext.Request.Query.TryGetValue(GoogleTag, out var res);
            var trackerValue = res.ToString();
            return trackerValue == null ? string.Empty : $"{GoogleTag}={trackerValue}";
        }
    }
}