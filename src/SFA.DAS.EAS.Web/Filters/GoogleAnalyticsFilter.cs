using System.Collections.Generic;
using System.Web.Mvc;
using SFA.DAS.EAS.Web.Controllers;

namespace SFA.DAS.EAS.Web.Filters
{
    public class GoogleAnalyticsFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string userId=null;
            string hashedAccountId = null;

            var thisController = filterContext.Controller as BaseController;
            if (thisController != null)
                userId = thisController.OwinWrapper.GetClaimValue(@"sub");

            if (filterContext.ActionParameters.ContainsKey("hashedAccountId"))
            {
                hashedAccountId = filterContext.ActionParameters["hashedAccountId"] as string;
            }

            filterContext.Controller.ViewBag.GaData = new GaData
            {           
                    UserId = userId,
                    Acc = hashedAccountId
            };

            base.OnActionExecuting(filterContext);
        }

        public string DataLoaded { get; set; }

        public class GaData
        {
            public string DataLoaded { get; set; } = "dataLoaded";
            public string UserId { get; set; }

            public string Vpv { get; set; }
            public string Acc { get; set; }

            public IDictionary<string, string> Extras { get; set; } = new Dictionary<string, string>();
        }
    }
}