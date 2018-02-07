using System;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Helpers;

namespace SFA.DAS.EAS.Web.Filters
{
    public class EnsureFeatureIsEnabledFilter : ActionFilterAttribute
    {
        private readonly Func<ICurrentUserService> _currentUserService;
        private readonly Func<IFeatureToggleService> _featureToggleService;

        public EnsureFeatureIsEnabledFilter(Func<ICurrentUserService> currentUserService, Func<IFeatureToggleService> featureToggleService)
        {
            _currentUserService = currentUserService;
            _featureToggleService = featureToggleService;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controllerName = (string)filterContext.RouteData.Values[ControllerConstants.ControllerKeyName];
            var actionName = (string)filterContext.RouteData.Values[ControllerConstants.ActionKeyName];
            var currentUser = _currentUserService().GetCurrentUser();
            var isFeatureEnabled = _featureToggleService().IsFeatureEnabled(controllerName, actionName, currentUser?.Email);

            if (!isFeatureEnabled)
            {
                filterContext.Result = new ViewResult
                {
                    ViewName = ControllerConstants.FeatureNotEnabledViewName,
                    ViewData = filterContext.Controller.ViewData,
                    TempData = filterContext.Controller.TempData
                };
            }
        }
    }
}