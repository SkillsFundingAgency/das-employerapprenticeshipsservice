using System.Web.Mvc;
using System.Web.Routing;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EmployerFinance.Web.Filters
{
    public class LevyEmployerTypeOnly : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var hashedAccountId = filterContext.ActionParameters["HashedAccountId"].ToString();
            var accountApi = DependencyResolver.Current.GetService<IAccountApiClient>();
            var account = accountApi.GetAccount(hashedAccountId).Result;
            if (account.ApprenticeshipEmployerType == "1")
            {
                base.OnActionExecuting(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(
                        new
                        {
                            controller = "AccessDenied",
                            action = "Index",
                        }));
            }
        }
    }
}