using System;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Helpers;

namespace SFA.DAS.EAS.Web.Filters
{
    public class ValidateFeatureFilter : ActionFilterAttribute
    {
        private readonly Func<IAuthorizationService> _authorizationService;

        public ValidateFeatureFilter(Func<IAuthorizationService> authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var isOperationAuthorised = _authorizationService().IsOperationAuthorized();

            if (!isOperationAuthorised)
            {
                filterContext.Result = new ViewResult { ViewName = ControllerConstants.FeatureNotEnabledViewName };
            }
        }
    }
}