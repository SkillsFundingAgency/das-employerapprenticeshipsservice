using Microsoft.Extensions.Configuration;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers;

public class CookieConsentController : Controller
{
    public IConfiguration Configuration { get; set; }

    public CookieConsentController(IConfiguration _configuration) 
    {
        Configuration = _configuration;
    }

    [HttpGet]
    [Route("cookieConsent", Order = 0)]
    [Route("cookieConsent/settings", Order = 1)]
    public IActionResult CookieConsent()
    {
        return Redirect(Url.EmployerAccountsAction("cookieConsent/settings", Configuration, false));
    }

    [HttpGet]
    [Route("accounts/{HashedAccountId}/cookieConsent", Order = 0)]
    [Route("accounts/{HashedAccountId}/cookieConsent/settings", Order = 1)]
    public IActionResult CookieConsentWithHashedAccountId()
    {
        return Redirect(Url.EmployerAccountsAction("cookieConsent/settings", Configuration, true));
    }
}