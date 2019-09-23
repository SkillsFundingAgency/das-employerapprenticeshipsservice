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
                AccountDetailViewModel account = null; 
                new Task(async () => account = await accountApi.GetAccount(hashedAccountId)).RunSynchronously();

                if (account.ApprenticeshipEmployerType == ((int)ApprenticeshipEmployerType.Levy).ToString())
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