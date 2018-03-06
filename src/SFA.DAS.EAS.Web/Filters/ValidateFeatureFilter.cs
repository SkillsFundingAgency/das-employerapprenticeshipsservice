using System;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authorization;
using SFA.DAS.EAS.Web.Helpers;

namespace SFA.DAS.EAS.Web.Filters
{
    public class ValidateFeatureFilter : ActionFilterAttribute
    {
        private readonly Func<IFeatureToggleService> _featureToggleService;
        private readonly Func<IAuthorizationService> _authorizationService;

        public ValidateFeatureFilter(Func<IFeatureToggleService> featureToggleService, Func<IAuthorizationService> authorizationService)
        {
            _featureToggleService = featureToggleService;
            _authorizationService = authorizationService;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controllerName = filterContext.RouteData.Values[ControllerConstants.ControllerKeyName].ToString();
            var actionName = filterContext.RouteData.Values[ControllerConstants.ActionKeyName].ToString();
            var authorizationContext = _authorizationService().GetAuthorizationContext();
            var isFeatureEnabled = _featureToggleService().IsFeatureEnabled(controllerName, actionName, authorizationContext);

            if (!isFeatureEnabled)
            {
                filterContext.Result = new ViewResult { ViewName = ControllerConstants.FeatureNotEnabledViewName };
            }
        }
    }
}