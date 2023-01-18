namespace SFA.DAS.EmployerAccounts.Web.Controllers;

public class BaseController : Controller
{
    
    private const string FlashMessageCookieName = "sfa-das-employerapprenticeshipsservice-flashmessage";

    private readonly ICookieStorageService<FlashMessageViewModel> _flashMessage;

    public BaseController(ICookieStorageService<FlashMessageViewModel> flashMessage)
    {
        _flashMessage = flashMessage;
    }

    public BaseController() { }
    
    public void AddFlashMessageToCookie(FlashMessageViewModel model)
    {
        _flashMessage.Delete(FlashMessageCookieName);

        _flashMessage.Create(model, FlashMessageCookieName);
    }

    public FlashMessageViewModel GetFlashMessageViewModelFromCookie()
    {
        var flashMessageViewModelFromCookie = _flashMessage.Get(FlashMessageCookieName);
        _flashMessage.Delete(FlashMessageCookieName);
        return flashMessageViewModelFromCookie;
    }

    /// <summary>
    /// Default implementation for the SupportUserBanner.  Can be overridden to render based on the available IAccountIdentifier model.
    /// </summary>
    public virtual ActionResult SupportUserBanner(IAccountIdentifier model = null)
    {
        return ViewComponent("SupportUserBanner", new SupportUserBannerViewModel());
    }
}