using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.FeatureToggle;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Controllers
{
    public class BaseController : Controller
    {
        private const string FlashMessageCookieName = "sfa-das-employerapprenticeshipsservice-flashmessage";

        private readonly IFeatureToggle _featureToggle;
        private readonly IMultiVariantTestingService _multiVariantTestingService;
        private readonly ICookieStorageService<FlashMessageViewModel> _flashMessage;
        protected IOwinWrapper OwinWrapper;
        

        public BaseController(
            IOwinWrapper owinWrapper, 
            IFeatureToggle featureToggle,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage )
        {
            OwinWrapper = owinWrapper;
            _featureToggle = featureToggle;
            _multiVariantTestingService = multiVariantTestingService;
            _flashMessage = flashMessage;
        }


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!CanAccessFeature())
            {
                filterContext.Result = base.View("FeatureNotEnabled", null, null);
            }
        }

        protected override ViewResult View(string viewName, string masterName, object model)
        {
            var orchestratorResponse = model as OrchestratorResponse;

            if (orchestratorResponse == null)
            {
                return base.View(viewName, masterName, model);
            }
            
            var invalidRequestException = orchestratorResponse.Exception as InvalidRequestException;

            if (invalidRequestException != null)
            {
                foreach (var errorMessageItem in invalidRequestException.ErrorMessages)
                {
                    ModelState.AddModelError(errorMessageItem.Key, errorMessageItem.Value);
                }
            }

            if (orchestratorResponse.Status == HttpStatusCode.OK || orchestratorResponse.Status == HttpStatusCode.BadRequest)
                return ReturnViewResult(viewName, masterName, orchestratorResponse);

            if (orchestratorResponse.Status == HttpStatusCode.Unauthorized)
            {
                //Get the account id
                var accountId = Request.Params["HashedAccountId"];
                if (accountId != null)
                {
                    ViewBag.AccountId = accountId;
                }

                return base.View(@"AccessDenied", masterName, orchestratorResponse);
            }

            if (orchestratorResponse.Status == HttpStatusCode.NotFound)
            {
                return base.View("NotFound");
            }

            return base.View(@"GenericError", masterName, orchestratorResponse);
        }

        private ViewResult ReturnViewResult(string viewName, string masterName, OrchestratorResponse orchestratorResponse)
        {

            var userViews = _multiVariantTestingService.GetMultiVariantViews();

            if (userViews == null)
            {
                return base.View(viewName, masterName, orchestratorResponse);
            }

            var controllerName = ControllerContext.RouteData.Values["Controller"].ToString();
            var actionName = ControllerContext.RouteData.Values["Action"].ToString();
            var userView = userViews.Data.SingleOrDefault(c => c.Controller.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase)
                            && c.Action.Equals(actionName, StringComparison.CurrentCultureIgnoreCase));

            if (userView != null)
            {
                if (!userView.SplitAccessAcrossUsers)
                {
                    var userEmail = OwinWrapper.GetClaimValue("email");

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


        private bool CanAccessFeature()
        {
            var features = _featureToggle.GetFeatures();
            if (features?.Data == null)
            {
                return true;
            }

            var controllerName = ControllerContext.RouteData.Values["Controller"].ToString();
            var controllerToggles = features.Data.Where(c => c.Controller.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase)).ToArray();
            if (!controllerToggles.Any())
            {
                return true;
            }

            var actionName = ControllerContext.RouteData.Values["Action"].ToString();
            var actionToggle = controllerToggles.Where(t => t.Action.Equals(actionName, StringComparison.CurrentCultureIgnoreCase) || t.Action == "*")
                                                 .OrderByDescending(t => t.Action) // Should put action = * last as specific action toggle should win
                                                 .FirstOrDefault();
            return actionToggle == null || IsUserInToggleWhiteList(actionToggle);
        }
        private bool IsUserInToggleWhiteList(FeatureToggleItem toggle)
        {
            if (toggle.WhiteList == null || toggle.WhiteList.Length == 0)
            {
                return false;
            }

            var userEmail = OwinWrapper.GetClaimValue("email");
            return toggle.WhiteList.Any(pattern => Regex.IsMatch(userEmail, pattern, RegexOptions.IgnoreCase));
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
    }
}