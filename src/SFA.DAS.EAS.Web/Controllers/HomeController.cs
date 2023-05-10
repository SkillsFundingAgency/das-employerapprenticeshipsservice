using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers;

[Route("service")]
public class HomeController : Controller
{
    private const string GoogleTag = "_ga";
    private readonly IConfiguration _configuration;

    public HomeController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [Route("~/")]
    [Route("")]
    [Route("Index")]
    public IActionResult Index()
    {
        return Redirect(Url.EmployerAccountsAction($"service/index?{GetTrackerQueryString()}", _configuration, false));
    }

    [Authorize(Policy = nameof(PolicyNames.HasUserAccount))]
    [HttpGet]
    [Route("accounts")]
    public IActionResult ViewAccounts()
    {
        return Redirect(Url.EmployerAccountsAction("service/accounts", _configuration, false));
    }

    [HttpGet]
    [Route("register")]
    public IActionResult RegisterUser()
    {
        return Redirect(Url.EmployerAccountsAction("service/register", _configuration, false));
    }

    [Authorize(Policy = nameof(PolicyNames.HasUserAccount))]
    [HttpGet]
    [Route("register/new")]
    public IActionResult HandleNewRegistration()
    {
        return Redirect(Url.EmployerAccountsAction("service/register/new", _configuration, false));
    }

    [Authorize(Policy = nameof(PolicyNames.HasUserAccount))]
    [HttpGet]
    [Route("password/change")]
    public IActionResult HandlePasswordChanged(bool userCancelled = false)
    {
        return Redirect(Url.EmployerAccountsAction("service/password/change", _configuration, false));
    }

    [Authorize(Policy = nameof(PolicyNames.HasUserAccount))]
    [HttpGet]
    [Route("email/change")]
    public IActionResult HandleEmailChanged(bool userCancelled = false)
    {
        return Redirect(Url.EmployerAccountsAction("service/email/change", _configuration, false));
    }

    [Authorize(Policy = nameof(PolicyNames.HasUserAccount))]
    [Route("signIn")]
    public IActionResult SignIn()
    {
        return Redirect(Url.EmployerAccountsAction("service/signIn", _configuration, false));
    }

    [Route("signOut")]
    public IActionResult SignOut()
    {
        return Redirect(Url.EmployerAccountsAction("service/signOut", _configuration, false));
    }

    [HttpGet]
    [Route("{HashedAccountId}/privacy", Order = 0)]
    [Route("privacy", Order = 1)]
    public IActionResult Privacy()
    {
        return Redirect(Url.EmployerAccountsAction("service/privacy", _configuration, false));
    }

    [HttpGet]
    [Route("cookieConsent")]
    public IActionResult CookieConsent()
    {
        return Redirect(Url.EmployerAccountsAction("cookieConsent/settings", _configuration, false));
    }

    [HttpGet]
    [Route("help")]
    public IActionResult Help()
    {
        return Redirect(Url.EmployerAccountsAction("service/help", _configuration, false));
    }

    [HttpGet]
    [Route("start")]
    public IActionResult ServiceStartPage()
    {
        return Redirect(Url.EmployerAccountsAction("service/start", _configuration, false));
    }

    [HttpGet]
    [Route("termsAndConditions/overview")]
    public IActionResult TermsAndConditionsOverview()
    {
        return Redirect(Url.EmployerAccountsAction("service/termsAndConditions/overview", _configuration, false));
    }

    private string GetTrackerQueryString()
    {
        Url.ActionContext.HttpContext.Request.Query.TryGetValue(GoogleTag, out var res);
        var trackerValue = res.ToString();
        return trackerValue == null ? string.Empty : $"{GoogleTag}={trackerValue}";
    }
}