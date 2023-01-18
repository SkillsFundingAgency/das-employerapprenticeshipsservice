using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerAccounts.Web.Infrastructure;

namespace SFA.DAS.EmployerAccounts.Web.Filters
{
    public class AnalyticsFilter : Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.Controller as Controller;
            if (controller != null)
            {
                var user = controller.User;
                var userId = user?.GetUserId();
                controller.ViewBag.GaData = new GaData
                {
                    UserId = userId,
                    Acc = controller.RouteData.Values[RouteValues.EncodedAccountId]?.ToString().ToUpper()
                };
            }
            base.OnActionExecuting(filterContext);
        }

        public class GaData
        {
            public string DataLoaded { get; set; } = "dataLoaded";
            public string UserId { get; set; }
            public string UserEmail { get; set; }
            public string UserName { get; set; }

            public string Vpv { get; set; }
            public string Acc { get; set; }

            public string AgreementId { get; set; }
            public string LevyFlag { get; set; }
        }
    }
}