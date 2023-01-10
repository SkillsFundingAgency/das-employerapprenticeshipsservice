using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web.Mvc;
using System.Web.Routing;
using SFA.DAS.Authorization.Mvc.Filters;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Web.Authorization;

namespace SFA.DAS.EmployerAccounts.Web
{
    public class DasEmployerAccountsAuthorizationFilter : AuthorizationFilter
    {
        private readonly EmployerAccountsConfiguration _config;
        public DasEmployerAccountsAuthorizationFilter(Func<IAuthorizationService> authorizationService, EmployerAccountsConfiguration config) : base(authorizationService)
        {
            _config = config;
        }
        
        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (filterContext.Result is HttpStatusCodeResult)
            {
                if (((Microsoft.AspNetCore.Mvc.StatusCodeResult)filterContext.Result).StatusCode.Equals((int)HttpStatusCode.Forbidden) &&
                    IsSupportConsoleUser(filterContext))
                {
                    if (filterContext.HttpContext.Request.RequestContext.RouteData.Values.TryGetValue(RouteValueKeys.AccountHashedId, out var accountHashedId))
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = $"accessdenied/{accountHashedId}" }));
                    }
                }
            }
        }

        private bool IsSupportConsoleUser(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext filterContext)
        {
            var requiredRoles = _config.SupportConsoleUsers.Split(',');
            return requiredRoles.Any(role => filterContext.HttpContext.User.IsInRole(role));
        }
    }
}