﻿using SFA.DAS.EmployerAccounts.Web.Filters;
using System.Web.Mvc;
using SFA.DAS.UnitOfWork.Mvc.Extensions;
using SFA.DAS.Authorization.Mvc.Filters;
using System;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EmployerAccounts.Web.Authorization;
using System.Net;
using System.Web.Routing;
using System.Security.Claims;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Extensions;

namespace SFA.DAS.EmployerAccounts.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.AddUnitOfWorkFilter();
            filters.Add(new AnalyticsFilter());            
            filters.AddDasEmployerAccountsAuthorizationFilter();
            filters.AddDasEmployerAccountsUnauthorizedAccessExceptionFilter();
        }
    }

    public class DasEmployerAccountsUnauthorizedAccessExceptionFilter : UnauthorizedAccessExceptionFilter
    {
        private readonly IUserContext _userContext;
        public DasEmployerAccountsUnauthorizedAccessExceptionFilter(IUserContext userContext)
        {
            _userContext = userContext;
        }
        public override void OnException(ExceptionContext filterContext) 
        {
            base.OnException(filterContext);
            
            if (filterContext.Exception is UnauthorizedAccessException)
            {
                
                if (((HttpStatusCodeResult)filterContext.Result).StatusCode.Equals((int)HttpStatusCode.Forbidden) &&
                    _userContext.IsSupportConsoleUser())
                {   
                    if (filterContext.HttpContext.Request.RequestContext.RouteData.Values.TryGetValue(RouteValueKeys.AccountHashedId, out var accountHashedId))
                    {
                        filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = $"accessdenied/{hashedAccountId}" }));

                    }
                    else
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "accessdenied" }));
                    }

                }
            }
        }
    }

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
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = $"accessdenied/{accountHashedId}" }));
                    }
                }
           }
        }
    }

    public static class GlobalFilterCollectionExtensions
    {
        public static void AddDasEmployerAccountsAuthorizationFilter(this GlobalFilterCollection filters)
        {
            filters.Add(new DasEmployerAccountsAuthorizationFilter(() => DependencyResolver.Current.GetService<IAuthorizationService>(), DependencyResolver.Current.GetService<IUserContext>()));
        }


        public static void AddDasEmployerAccountsUnauthorizedAccessExceptionFilter(this GlobalFilterCollection filters)
        {
            filters.Add(new DasEmployerAccountsUnauthorizedAccessExceptionFilter(DependencyResolver.Current.GetService<IUserContext>()));
        }
    }
}
