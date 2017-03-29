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
        private readonly IFeatureToggle _featureToggle;
        protected IOwinWrapper OwinWrapper;

        public BaseController(
            IOwinWrapper owinWrapper, 
            IFeatureToggle featureToggle)
        {
            OwinWrapper = owinWrapper;
            _featureToggle = featureToggle;
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

            if (orchestratorResponse == null) return base.View(viewName, masterName, model);

            var flashMessage = GetHomePageSucessMessage();
            if (flashMessage != null)
            {
                orchestratorResponse.FlashMessage = flashMessage;
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

        protected FlashMessageViewModel GetHomePageSucessMessage()
        {
            if (TempData.ContainsKey("successHeader") || TempData.ContainsKey("successMessage"))
            {
                var successMessageViewModel = new FlashMessageViewModel();
                object message;
                successMessageViewModel.Severity = FlashMessageSeverityLevel.Success;
                if (TempData.TryGetValue("successHeader", out message))
                {
                    successMessageViewModel.Headline = message.ToString();
                }
                if (TempData.TryGetValue("successCompany", out message))
                {
                    successMessageViewModel.Message = message.ToString();
                }
                if (TempData.TryGetValue("successMessage", out message))
                {
                    successMessageViewModel.SubMessage = message.ToString();
                }
                return successMessageViewModel;
            }
            return null;
        }
    }
}