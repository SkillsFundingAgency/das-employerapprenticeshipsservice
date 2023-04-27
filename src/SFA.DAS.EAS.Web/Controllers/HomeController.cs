using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EAS.Web.Controllers;

[Route("service")]
public class HomeController : Controller
{
    private const string GoogleTag = "_ga";
    public IConfiguration Configuration { get; set; }
    public HomeController(IConfiguration _configuration)
    {
        Configuration = _configuration;
    }

    [Route("~/")]
    [Route("")]
    [Route("Index")]
    public IActionResult Index()
    {
        return Redirect(Url.EmployerAccountsAction($"service/index?{GetTrackerQueryString()}", Configuration, false));
    }        

    [AuthoriseActiveUser]
    [HttpGet]
    [Route("accounts")]
    public IActionResult ViewAccounts()
    {
        return Redirect(Url.EmployerAccountsAction("service/accounts", Configuration, false));
    }

    [HttpGet]
    [Route("register")]
    public IActionResult RegisterUser()
    {
        return Redirect(Url.EmployerAccountsAction("service/register", Configuration, false));
    }

    [DasAuthorize]
    [HttpGet]
    [Route("register/new")]
    public IActionResult HandleNewRegistration()
    {
        return Redirect(Url.EmployerAccountsAction("service/register/new", Configuration, false));
    }

    [DasAuthorize]
    [HttpGet]
    [Route("password/change")]
    public IActionResult HandlePasswordChanged(bool userCancelled = false)
    {
        return Redirect(Url.EmployerAccountsAction("service/password/change", Configuration, false));
    }

    [DasAuthorize]
    [HttpGet]
    [Route("email/change")]
    public IActionResult HandleEmailChanged(bool userCancelled = false)
    {
        return Redirect(Url.EmployerAccountsAction("service/email/change", Configuration, false));
    }

    [DasAuthorize]
    [Route("signIn")]
    public IActionResult SignIn()
    {
        return Redirect(Url.EmployerAccountsAction("service/signIn", Configuration, false));
    }

    [Route("signOut")]
    public IActionResult SignOut()
    {
        return Redirect(Url.EmployerAccountsAction("service/signOut", Configuration, false));
    }

    [HttpGet]
    [Route("{HashedAccountId}/privacy", Order = 0)]
    [Route("privacy", Order = 1)]
    public IActionResult Privacy()
    {
        return Redirect(Url.EmployerAccountsAction("service/privacy", Configuration, false));
    }

    [HttpGet]
    [Route("cookieConsent")]
    public IActionResult CookieConsent()
    {
        return Redirect(Url.EmployerAccountsAction("cookieConsent/settings", Configuration, false));
    }

    [HttpGet]
    [Route("help")]
    public IActionResult Help()
    {
        return Redirect(Url.EmployerAccountsAction("service/help", Configuration, false));
    }

    [HttpGet]
    [Route("start")]
    public IActionResult ServiceStartPage()
    {
        return Redirect(Url.EmployerAccountsAction("service/start", Configuration, false));
    }

    [HttpGet]
    [Route("termsAndConditions/overview")]
    public IActionResult TermsAndConditionsOverview()
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