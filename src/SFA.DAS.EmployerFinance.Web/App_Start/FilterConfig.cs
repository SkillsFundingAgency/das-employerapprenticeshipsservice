using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using NLog;
using SFA.DAS.Authorization.Mvc.Extensions;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EmployerFinance.Web.Filters;
using SFA.DAS.UnitOfWork.Mvc.Extensions;

namespace SFA.DAS.EmployerFinance.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.AddUnitOfWorkFilter();
            //filters.AddAuthorizationFilter();
            filters.Add(new AuthorizationFilterLocal(() => DependencyResolver.Current.GetService<IAuthorizationService>()));
            filters.AddUnauthorizedAccessExceptionFilter();
            filters.Add(new AnalyticsFilter());
        }
    }

    public class AuthorizationFilterLocal : ActionFilterAttribute
    {
        private readonly Func<IAuthorizationService> _authorizationService;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public AuthorizationFilterLocal(Func<IAuthorizationService> authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                var dasAuthorizeAttributes = filterContext.ActionDescriptor.GetDasAuthorizeAttributes();

                if (dasAuthorizeAttributes.Count > 0)
                {
                    try
                    {
                        var options = dasAuthorizeAttributes.SelectMany(a => a.Options).ToArray();
                        var isAuthorized = _authorizationService().IsAuthorized(options);

                        if (!isAuthorized)
                        {
                            filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "AuthorizationFilterLocal::2");
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "AuthorizationFilterLocal::1");
                throw;
            }
        }
    }
}
