using System;
using System.Web.Mvc;
using System.Web.Routing;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerFinance.Web.Helpers;

namespace SFA.DAS.EmployerFinance.Web.Filters
{
    public class LevyEmployerTypeOnly : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                var hashedAccountId = filterContext.ActionParameters["HashedAccountId"].ToString();
                var accountApi = DependencyResolver.Current.GetService<IAccountApiClient>();
                var task = accountApi.GetAccount(hashedAccountId);
                task.RunSynchronously();
                var account = task.Result;

                if (account.ApprenticeshipEmployerType == "1")
                {
                    base.OnActionExecuting(filterContext);
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(
                            new
                            {
                                controller = ControllerConstants.AccessDeniedControllerName,
                                action = "Index",
                            }));
                }
            }
            catch (Exception ex)
            {
                filterContext.Result = new ViewResult { ViewName = ControllerConstants.BadRequestViewName };
            }
        }
    }
}