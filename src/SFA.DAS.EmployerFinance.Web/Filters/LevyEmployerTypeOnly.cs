using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerFinance.Web.Helpers;

namespace SFA.DAS.EmployerFinance.Web.Filters
{
    public class LevyEmployerTypeOnly : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if(filterContext.ActionParameters == null || !filterContext.ActionParameters.ContainsKey("HashedAccountId"))
                {
                    filterContext.Result = new ViewResult { ViewName = ControllerConstants.BadRequestViewName };
                    return;
                }

                var hashedAccountId = filterContext.ActionParameters["HashedAccountId"].ToString();
                var accountApi = DependencyResolver.Current.GetService<IAccountApiClient>();
                
                var task = Task.Run(async () => await accountApi.GetAccount(hashedAccountId));
                AccountDetailViewModel account = task.Result;
                ApprenticeshipEmployerType apprenticeshipEmployerType = (ApprenticeshipEmployerType)Enum.Parse(typeof(ApprenticeshipEmployerType), account.ApprenticeshipEmployerType, true);

                if (apprenticeshipEmployerType == ApprenticeshipEmployerType.Levy)
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