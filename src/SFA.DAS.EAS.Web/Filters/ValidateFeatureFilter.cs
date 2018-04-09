using System;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Helpers;

namespace SFA.DAS.EAS.Web.Filters
{
    public class ValidateFeatureFilter : ActionFilterAttribute
    {
        private readonly Func<IAuthorizationService> _operationAuthorisationService;
        private readonly Func<IAuthorizationService> _authorizationService;

        public ValidateFeatureFilter(Func<IAuthorizationService> operationAuthorisationService, Func<IAuthorizationService> authorizationService)
        {
            _operationAuthorisationService = operationAuthorisationService;
            _authorizationService = authorizationService;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var authorizationContext = _authorizationService().GetAuthorizationContext();
            var isOperationAuthorised = _operationAuthorisationService().IsOperationAuthorised(authorizationContext);

            if (!isOperationAuthorised)
            {
                filterContext.Result = new ViewResult { ViewName = ControllerConstants.FeatureNotEnabledViewName };
            }
        }
    }
}