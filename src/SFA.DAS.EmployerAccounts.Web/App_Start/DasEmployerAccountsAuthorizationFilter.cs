using System;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;
using SFA.DAS.Authorization.Mvc.Filters;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EmployerAccounts.Web.Authorization;

namespace SFA.DAS.EmployerAccounts.Web
{
    public class DasEmployerAccountsAuthorizationFilter : AuthorizationFilter
    {
        private readonly IUserContext _userContext;
        public DasEmployerAccountsAuthorizationFilter(Func<IAuthorizationService> authorizationService, IUserContext userContext) : base(authorizationService)
        {
            _userContext = userContext;
        }
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (filterContext.Result is HttpStatusCodeResult)
            {
                if (((HttpStatusCodeResult)filterContext.Result).StatusCode.Equals((int)HttpStatusCode.Forbidden) &&
                    _userContext.IsSupportConsoleUser())
                {
                    if (filterContext.HttpContext.Request.RequestContext.RouteData.Values.TryGetValue(RouteValueKeys.AccountHashedId, out var accountHashedId))
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "SupportError", action = $"accessdenied/{accountHashedId}" }));
                    }
                }
            }
        }
    }
}