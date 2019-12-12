using SFA.DAS.EmployerAccounts.Web.Filters;
using System.Web.Mvc;
using SFA.DAS.UnitOfWork.Mvc.Extensions;
using SFA.DAS.Authorization.Mvc.Filters;
using System;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EmployerAccounts.Web.Authorization;
using System.Net;
using System.Web.Routing;

namespace SFA.DAS.EmployerAccounts.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.AddUnitOfWorkFilter();
            filters.Add(new GoogleAnalyticsFilter());            
            filters.AddDasEmployerAccountsAuthorizationFilter();
            filters.AddDasEmployerAccountsUnauthorizedAccessExceptionFilter();            
        }
    }

    public class DasEmployerAccountsUnauthorizedAccessExceptionFilter : UnauthorizedAccessExceptionFilter
    {
        public override void OnException(ExceptionContext filterContext) 
        {
            base.OnException(filterContext);
            if (filterContext.Exception is UnauthorizedAccessException)
            {
                if (((HttpStatusCodeResult)filterContext.Result).StatusCode.Equals((int)HttpStatusCode.Forbidden) && filterContext.HttpContext.User.IsInRole(AuthorizationConstants.Tier2User))
                {
                    if (filterContext.HttpContext.Request.RequestContext.RouteData.Values.TryGetValue(RouteValueKeys.AccountHashedId, out var accountHashedId))
                    {
                        //filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = $"accessdenied",  hashedAccountId = accountHashedId  }));
                        //filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(new RouteValueDictionary(new { controller = "Restrict", action = $"Refuse", hashedAccountId = accountHashedId }));
                        filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = $"accessdenied/{accountHashedId}" }));
                    }
                }
            }
        }
    }

    public class DasEmployerAccountsAuthorizationFilter : AuthorizationFilter
    {       
        public DasEmployerAccountsAuthorizationFilter(Func<IAuthorizationService> authorizationService) : base(authorizationService)
        {           
        }
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

           if (filterContext.Result is HttpStatusCodeResult)
           {
                if (((HttpStatusCodeResult)filterContext.Result).StatusCode.Equals((int)HttpStatusCode.Forbidden) && filterContext.HttpContext.User.IsInRole(AuthorizationConstants.Tier2User))
                {
                    if (filterContext.HttpContext.Request.RequestContext.RouteData.Values.TryGetValue(RouteValueKeys.AccountHashedId, out var accountHashedId))
                    {
                        //filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(new RouteValueDictionary(new { controller = "Restrict", action = $"Refuse", hashedAccountId = accountHashedId }));                        
                        filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = $"accessdenied/{accountHashedId}" }));

                    }
                }
                
            }

        }
    }

    public static class GlobalFilterCollectionExtensions
    {
        public static void AddDasEmployerAccountsAuthorizationFilter(this GlobalFilterCollection filters)
        {
            filters.Add(new DasEmployerAccountsAuthorizationFilter(() => DependencyResolver.Current.GetService<IAuthorizationService>()));
        }


        public static void AddDasEmployerAccountsUnauthorizedAccessExceptionFilter(this GlobalFilterCollection filters)
        {
            filters.Add(new DasEmployerAccountsUnauthorizedAccessExceptionFilter());
        }
    }
}
