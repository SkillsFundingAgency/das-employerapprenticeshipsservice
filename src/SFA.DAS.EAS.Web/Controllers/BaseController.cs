using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Controllers
{
    public class BaseController : Controller
    {
        private readonly IFeatureToggle _featureToggle;
        private readonly IUserWhiteList _userWhiteList;
        protected IOwinWrapper OwinWrapper;

        public BaseController(IOwinWrapper owinWrapper, IFeatureToggle featureToggle, IUserWhiteList userWhiteList)
        {
            OwinWrapper = owinWrapper;
            _featureToggle = featureToggle;
            _userWhiteList = userWhiteList;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!CheckFeatureIsEnabled())
            {
                filterContext.Result = base.View("FeatureNotEnabled", null, null);
            }
            if (filterContext.ActionDescriptor.IsDefined (typeof(AuthorizeAttribute), true) || (filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AuthorizeAttribute), true)) && !filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                // Check for authorization
                var userEmail = OwinWrapper.GetClaimValue("email");

                if (!string.IsNullOrEmpty(userEmail))
                {
                    if (!_userWhiteList.IsEmailOnWhiteList(userEmail))
                    {
                        filterContext.Result = base.View("UserNotAllowed", null, null);
                    }
                }
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
                return base.View(viewName, masterName, orchestratorResponse);

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

            return base.View(@"GenericError", masterName, orchestratorResponse);
        }

        private bool CheckFeatureIsEnabled()
        {
            var features = _featureToggle.GetFeatures();
            if (features?.Data == null)
            {
                return true;
            }

            var controllerName = ControllerContext.RouteData.Values["Controller"].ToString();
            var actionName = ControllerContext.RouteData.Values["Action"].ToString();

            var featureToggleItems = features.Data.Where(c => c.Controller.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase));

            return featureToggleItems.All(featureToggleItem => featureToggleItem.Action != "*" && !actionName.Equals(featureToggleItem.Action, StringComparison.CurrentCultureIgnoreCase));
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