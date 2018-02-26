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
        private readonly Func<IMembershipService> _membershipService;

        public ValidateFeatureFilter(Func<IFeatureToggleService> featureToggleService, Func<IMembershipService> membershipService)
        {
            _featureToggleService = featureToggleService;
            _membershipService = membershipService;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controllerName = filterContext.RouteData.Values[ControllerConstants.ControllerKeyName].ToString();
            var actionName = filterContext.RouteData.Values[ControllerConstants.ActionKeyName].ToString();
            var membershipContext = _membershipService().GetMembershipContext();
            var isFeatureEnabled = _featureToggleService().IsFeatureEnabled(controllerName, actionName, membershipContext);

            if (!isFeatureEnabled)
            {
                filterContext.Result = new ViewResult { ViewName = ControllerConstants.FeatureNotEnabledViewName };
            }
        }
    }
}