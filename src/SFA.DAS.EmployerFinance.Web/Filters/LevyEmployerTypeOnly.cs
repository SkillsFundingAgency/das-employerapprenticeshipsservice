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
                string hashedAccountId = string.Empty;

                if(filterContext.ActionParameters != null && filterContext.ActionParameters.ContainsKey("HashedAccountId"))
                {
                    hashedAccountId = filterContext.ActionParameters["HashedAccountId"].ToString();

                } else if(filterContext.RouteData?.Values?.ContainsKey("HashedAccountId") == true) {

                    hashedAccountId = filterContext.RouteData.Values["HashedAccountId"].ToString();
                }

                if(string.IsNullOrWhiteSpace(hashedAccountId))
                {
                    filterContext.Result = new ViewResult { ViewName = ControllerConstants.BadRequestViewName };
                    return;
                }
                
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
                                hashedAccountId = hashedAccountId
                            }));
                }
            }
            catch (Exception)
            {
                filterContext.Result = new ViewResult { ViewName = ControllerConstants.BadRequestViewName };
            }
        }
    }
}