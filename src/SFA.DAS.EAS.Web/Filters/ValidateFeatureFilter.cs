using System;
using System.Net;
using System.Web.Mvc;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Infrastructure.Features;
using SFA.DAS.EAS.Web.Extensions;

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
            var featureAttribute = filterContext.ActionDescriptor.GetCustomAttribute<FeatureAttribute>();

            if (featureAttribute != null)
            {
                var featureType = featureAttribute.FeatureType;
                var isAuthorized = _authorizationService().IsAuthorized(featureType);

                if (!isAuthorized)
                {
                    filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }
        }
    }
}