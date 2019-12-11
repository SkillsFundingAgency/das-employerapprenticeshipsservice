using SFA.DAS.EmployerAccounts.Web.Filters;
using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc.Extensions;
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
            filters.AddEasAuthorizationFilter();
            filters.AddEasUnauthorizedAccessExceptionFilter();            
        }
    }

    public class EasUnauthorizedAccessExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {            
            if (filterContext.Exception is UnauthorizedAccessException)
            {               
                if (filterContext.HttpContext.User.IsInRole(AuthorizationConstants.Tier2User))
                {
                    if (filterContext.HttpContext.Request.RequestContext.RouteData.Values.TryGetValue(RouteValueKeys.AccountHashedId, out var accountHashedId))
                    {
                        filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = $"accessdenied/{accountHashedId}" }));
                    }
                }
                else
                {
                    filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                    filterContext.ExceptionHandled = true;
                }
               
            }
        }
    }

    public class EasAuthorizationFilter : AuthorizationFilter
    {       
        public EasAuthorizationFilter(Func<IAuthorizationService> authorizationService) : base(authorizationService)
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
                        filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = $"accessdenied/{accountHashedId}" }));
                    }
                }
                
            }

        }
    }

    public static class GlobalFilterCollectionExtensions
    {
        public static void AddEasAuthorizationFilter(this GlobalFilterCollection filters)
        {
            filters.Add(new EasAuthorizationFilter(() => DependencyResolver.Current.GetService<IAuthorizationService>()));
        }


        public static void AddEasUnauthorizedAccessExceptionFilter(this GlobalFilterCollection filters)
        {
            filters.Add(new EasUnauthorizedAccessExceptionFilter());
        }
    }
}
