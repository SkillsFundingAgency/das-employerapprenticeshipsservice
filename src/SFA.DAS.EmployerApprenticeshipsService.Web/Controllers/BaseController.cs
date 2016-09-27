using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
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
            if (orchestratorResponse.Status == HttpStatusCode.OK || orchestratorResponse.Status == HttpStatusCode.BadRequest)
                return base.View(viewName, masterName, orchestratorResponse);

            if (orchestratorResponse.Status == HttpStatusCode.Unauthorized)
            {
                //Get the account id
                var accountId = Request.Params["AccountId"];
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
            var controllerName = ControllerContext.RouteData.Values["Controller"].ToString();
            var actionName = ControllerContext.RouteData.Values["Action"].ToString();

            var featureToggleItem = features.Data.FirstOrDefault(c => c.Controller.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase));
            if (featureToggleItem != null)
            {
                if (featureToggleItem.Action == "*" || actionName.Equals(featureToggleItem.Action, StringComparison.CurrentCultureIgnoreCase))
                {
                    return false;
                }
            }

            return true;
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