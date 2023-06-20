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


    [NonAction]
    public override ViewResult View(string viewName, object model)
    {
        if (!(model is OrchestratorResponse orchestratorResponse))
        {
            return base.View(viewName, model);
        }

        if (orchestratorResponse.Exception is InvalidRequestException invalidRequestException)
        {
            foreach (var errorMessageItem in invalidRequestException.ErrorMessages)
            {
                ModelState.AddModelError(errorMessageItem.Key, errorMessageItem.Value);
            }

            return base.View(viewName, orchestratorResponse);
        }

        if (orchestratorResponse.Status == HttpStatusCode.BadRequest)
        {
            return base.View(viewName, orchestratorResponse);
        }

        if (orchestratorResponse.Status == HttpStatusCode.NotFound)
        {
            return base.View(ControllerConstants.NotFoundViewName);
        }

        if (orchestratorResponse.Status == HttpStatusCode.OK)
        {
            return base.View(viewName, orchestratorResponse);
        }

        if (orchestratorResponse.Status == HttpStatusCode.Unauthorized)
        {
            var accountId = Request.Query[ControllerConstants.AccountHashedIdRouteKeyName].ToString();

            if (accountId != null)
            {
                ViewBag.AccountId = accountId;
            }

            return base.View(ControllerConstants.AccessDeniedViewName, orchestratorResponse);
        }

        if (orchestratorResponse.Exception != null)
        {
            throw orchestratorResponse.Exception;
        }

        throw new OrchestratorResponseTypeException(model.GetType());
    }

    [NonAction]
    public void AddFlashMessageToCookie(FlashMessageViewModel model)
    {
        _flashMessage.Delete(FlashMessageCookieName);

        _flashMessage.Create(model, FlashMessageCookieName);
    }

    [NonAction]
    public FlashMessageViewModel GetFlashMessageViewModelFromCookie()
    {
        var flashMessageViewModelFromCookie = _flashMessage.Get(FlashMessageCookieName);
        _flashMessage.Delete(FlashMessageCookieName);
        return flashMessageViewModelFromCookie;
    }

    /// <summary>
    /// Default implementation for the SupportUserBanner.  Can be overridden to render based on the available IAccountIdentifier model.
    /// </summary>
    [NonAction]
    public virtual IActionResult SupportUserBanner(IAccountIdentifier model = null)
    {
        return ViewComponent("SupportUserBanner", new SupportUserBannerViewModel());
    }
}

[Serializable]
public class OrchestratorResponseTypeException : Exception
{
    public OrchestratorResponseTypeException(Type modelType) : base($"Orchestrator response of type '{modelType}' could not be handled.") { }
}