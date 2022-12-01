using System.Collections.Generic;
using System.Web.Mvc;
using SFA.DAS.EmployerFinance.Web.Controllers;

namespace SFA.DAS.EmployerFinance.Web.Filters
{
    public class AnalyticsFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string userId = null;
            string hashedAccountId = null;
            string userEmail = null;
            string userName = null;

            var thisController = filterContext.Controller as BaseController;
            if (thisController != null)
            {
                userId = thisController.OwinWrapper.GetClaimValue(@"sub");
                userEmail = thisController.OwinWrapper.GetClaimValue(@"email");
                userName = $"{thisController.OwinWrapper.GetClaimValue(@"firstname")} {thisController.OwinWrapper.GetClaimValue(@"lastname")}";
            }

            if (filterContext.ActionParameters.ContainsKey("hashedAccountId"))
            {
                hashedAccountId = filterContext.ActionParameters["hashedAccountId"] as string;
            }
            else if(filterContext.Controller.ControllerContext.RouteData.Values.ContainsKey("hashedAccountId"))
            {
                hashedAccountId = filterContext.Controller.ControllerContext.RouteData.Values["hashedAccountId"] as string;
            }

            filterContext.Controller.ViewBag.GaData = new GaData
            {
                UserId = userId,
                Acc = hashedAccountId,
                UserEmail = userEmail,
                UserName = userName
            };

            base.OnActionExecuting(filterContext);
        }

        public string DataLoaded { get; set; }

        public class GaData
        {
            public GaData()
            {
            }

            public string DataLoaded { get; set; } = "dataLoaded";
            public string UserId { get; set; }
            public string UserEmail { get; set; }
            public string UserName { get; set; }
            public string Vpv { get; set; }
            public string Acc { get; set; }

            public IDictionary<string, string> Extras { get; set; } = new Dictionary<string, string>();
        }
    }
}