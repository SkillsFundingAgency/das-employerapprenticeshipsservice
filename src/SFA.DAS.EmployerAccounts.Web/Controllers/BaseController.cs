using System.Security.Claims;
using System.Text.RegularExpressions;

namespace SFA.DAS.EmployerAccounts.Web.Controllers;

public class BaseController : Controller
{

    private const string FlashMessageCookieName = "sfa-das-employerapprenticeshipsservice-flashmessage";

    private readonly ICookieStorageService<FlashMessageViewModel> _flashMessage;
    private readonly IMultiVariantTestingService _multiVariantTestingService;

    public BaseController(ICookieStorageService<FlashMessageViewModel> flashMessage, IMultiVariantTestingService multiVariantTestingService)
    {
        _flashMessage = flashMessage;
        _multiVariantTestingService = multiVariantTestingService;
    }

    public BaseController() { }


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

            return ReturnViewResult(viewName, orchestratorResponse);
        }

        if (orchestratorResponse.Status == HttpStatusCode.BadRequest)
        {
            return ReturnViewResult(viewName, orchestratorResponse);
        }

        if (orchestratorResponse.Status == HttpStatusCode.NotFound)
        {
            return base.View(ControllerConstants.NotFoundViewName);
        }

        if (orchestratorResponse.Status == HttpStatusCode.OK)
        {
            return ReturnViewResult(viewName, orchestratorResponse);
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

        throw new Exception($"Orchestrator response of type '{model.GetType()}' could not be handled.");
    }

    private ViewResult ReturnViewResult(string viewName, OrchestratorResponse orchestratorResponse)
    {

        var userViews = _multiVariantTestingService.GetMultiVariantViews();

        if (userViews == null)
        {
            return base.View(viewName, orchestratorResponse);
        }

        var controllerName = ControllerContext.RouteData.Values[ControllerConstants.ControllerKeyName].ToString();
        var actionName = ControllerContext.RouteData.Values[ControllerConstants.ActionKeyName].ToString();
        var userView = userViews.Data.SingleOrDefault(c => c.Controller.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase)
                        && c.Action.Equals(actionName, StringComparison.CurrentCultureIgnoreCase));

        if (userView != null)
        {
            if (!userView.SplitAccessAcrossUsers)
            {
                var userEmail = HttpContext.User.FindFirstValue(ControllerConstants.EmailClaimKeyName);

                foreach (var view in userView.Views)
                {
                    if (view.EmailAddresses.Any(pattern => Regex.IsMatch(userEmail, pattern, RegexOptions.IgnoreCase)))
                    {
                        return base.View(view.ViewName, orchestratorResponse);
                    }
                }
            }
            else
            {
                var randomViewName = _multiVariantTestingService.GetRandomViewNameToShow(userView.Views);

                if (string.IsNullOrEmpty(randomViewName))
                {
                    return base.View(viewName, orchestratorResponse);
                }

                return base.View(randomViewName, orchestratorResponse);
            }
        }

        return base.View(viewName, orchestratorResponse);
    }

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
    public virtual IActionResult SupportUserBanner(IAccountIdentifier model = null)
    {
        return ViewComponent("SupportUserBanner", new SupportUserBannerViewModel());
    }
}