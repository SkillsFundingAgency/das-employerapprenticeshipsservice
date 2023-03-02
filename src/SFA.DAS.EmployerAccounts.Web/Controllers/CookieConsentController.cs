namespace SFA.DAS.EmployerAccounts.Web.Controllers;

public class CookieConsentController : BaseController
{
    public CookieConsentController(
        ICookieStorageService<FlashMessageViewModel> flashMessage
    ) : base(flashMessage) { }

    [HttpGet]
    [Route("accounts/{HashedAccountId}/cookieConsent", Order = 0)]
    [Route("cookieConsent", Order = 1)]        
    public IActionResult Settings()
    {
        return View();
    }

    [HttpGet]
    [Route("accounts/{HashedAccountId}/cookieConsent/details", Order = 0)]
    [Route("cookieConsent/details", Order = 1)]
    public IActionResult Details()
    {
        return View();
    }
}