using System;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Helpers;

namespace SFA.DAS.EAS.Web.Filters
{
    public class EnsureFeatureIsEnabledFilter : ActionFilterAttribute
    {
        private readonly Func<ICurrentUserService> _currentUserServiceFactory;
        private readonly Func<IFeatureToggleService> _featureToggleServiceFactory;

        public EnsureFeatureIsEnabledFilter(Func<ICurrentUserService> currentUserServiceFactory, Func<IFeatureToggleService> featureToggleServiceFactory)
        {
            _currentUserServiceFactory = currentUserServiceFactory;
            _featureToggleServiceFactory = featureToggleServiceFactory;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controllerName = (string)filterContext.RouteData.Values[ControllerConstants.ControllerKeyName];
            var actionName = (string)filterContext.RouteData.Values[ControllerConstants.ActionKeyName];
            var currentUser = _currentUserServiceFactory().GetCurrentUser();

            if (!_featureToggleServiceFactory().IsFeatureEnabled(controllerName, actionName, currentUser?.ExternalUserId, currentUser?.Email))
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