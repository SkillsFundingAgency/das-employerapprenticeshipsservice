using System.Linq;
using System.Text.RegularExpressions;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Web.Controllers;

public class BaseController : Controller
{
    public IAuthenticationService OwinWrapper;

    private const string FlashMessageCookieName = "sfa-das-employerapprenticeshipsservice-flashmessage";

    private readonly IMultiVariantTestingService _multiVariantTestingService;
    private readonly ICookieStorageService<FlashMessageViewModel> _flashMessage;

    public BaseController(IAuthenticationService owinWrapper, IMultiVariantTestingService multiVariantTestingService, ICookieStorageService<FlashMessageViewModel> flashMessage)
    {
        OwinWrapper = owinWrapper;
        _multiVariantTestingService = multiVariantTestingService;
        _flashMessage = flashMessage;
    }

    public BaseController(IAuthenticationService owinWrapper)
    {
        OwinWrapper = owinWrapper;
    }

    protected override Microsoft.AspNetCore.Mvc.ViewResult View(string viewName, string masterName, object model)
    {
        if (!(model is OrchestratorResponse orchestratorResponse))
        {
            return base.View(viewName, masterName, model);
        }
            
        if (orchestratorResponse.Exception is InvalidRequestException invalidRequestException)
        {
            foreach (var errorMessageItem in invalidRequestException.ErrorMessages)
            {
                ModelState.AddModelError(errorMessageItem.Key, errorMessageItem.Value);
            }

            return ReturnViewResult(viewName, masterName, orchestratorResponse);
        }

        if (orchestratorResponse.Status == HttpStatusCode.BadRequest)
        {
            return ReturnViewResult(viewName, masterName, orchestratorResponse);
        }

        if (orchestratorResponse.Status == HttpStatusCode.NotFound)
        {
            return base.View(ControllerConstants.NotFoundViewName);
        }

        if (orchestratorResponse.Status == HttpStatusCode.OK)
        {
            return ReturnViewResult(viewName, masterName, orchestratorResponse);
        }

        if (orchestratorResponse.Status == HttpStatusCode.Unauthorized)
        {
            var accountId = Request.Params[ControllerConstants.AccountHashedIdRouteKeyName];

            if (accountId != null)
            {
                ViewBag.AccountId = accountId;
            }

            return base.View(ControllerConstants.AccessDeniedViewName, masterName, orchestratorResponse);
        }

        if (orchestratorResponse.Exception != null)
        {
            throw orchestratorResponse.Exception;
        }

        throw new Exception($"Orchestrator response of type '{model.GetType()}' could not be handled.");
    }

    private Microsoft.AspNetCore.Mvc.ViewResult ReturnViewResult(string viewName, string masterName, OrchestratorResponse orchestratorResponse)
    {

        var userViews = _multiVariantTestingService.GetMultiVariantViews();

        if (userViews == null)
        {
            return base.View(viewName, masterName, orchestratorResponse);
        }

        var controllerName = ControllerContext.RouteData.Values[ControllerConstants.ControllerKeyName].ToString();
        var actionName = ControllerContext.RouteData.Values[ControllerConstants.ActionKeyName].ToString();
        var userView = userViews.Data.SingleOrDefault(c => Microsoft.AspNetCore.Mvc.Controller.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase)
                                                           && c.Action.Equals(actionName, StringComparison.CurrentCultureIgnoreCase));

        if (userView != null)
        {
            if (!userView.SplitAccessAcrossUsers)
            {
                var userEmail = OwinWrapper.GetClaimValue(ControllerConstants.EmailClaimKeyName);

                foreach (var view in userView.Views)
                {
                    if (view.EmailAddresses.Any(pattern => Regex.IsMatch(userEmail, pattern, RegexOptions.IgnoreCase)))
                    {
                        return base.View(view.ViewName, masterName, orchestratorResponse);
                    }
                }
            }
            else
            {
                var randomViewName = _multiVariantTestingService.GetRandomViewNameToShow(userView.Views);

                if (string.IsNullOrEmpty(randomViewName))
                {
                    return base.View(viewName, masterName, orchestratorResponse);
                }

                return base.View(randomViewName, masterName, orchestratorResponse);
            }
        }

        return base.View(viewName, masterName, orchestratorResponse);
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

    public void RemoveFlashMessageFromCookie()
    {
        _flashMessage.Delete(FlashMessageCookieName);
    }

    /// <summary>
    /// Default implementation for the SupportUserBanner.  Can be overridden to render based on the available IAccountIdentifier model.
    /// </summary>
    public virtual Microsoft.AspNetCore.Mvc.ActionResult SupportUserBanner(IAccountIdentifier model = null)
    {
        return PartialView("_SupportUserBanner", new SupportUserBannerViewModel());
    }
}