using System.Collections.Generic;
using System.Web.Mvc;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Filters
{
    public class AnalyticsFilter : Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext filterContext)
        {
            string userId = null;
            string hashedAccountId = null;
            string agreementId = null;
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

            if (filterContext.ActionParameters.ContainsKey("agreementId"))
            {
                agreementId = filterContext.ActionParameters["agreementId"] as string;
            }

            Microsoft.AspNetCore.Mvc.Controller.ViewBag.GaData = new GaData
            {
                UserId = userId,
                Acc = hashedAccountId,
                AgreementId = agreementId,
                UserEmail = userEmail,
                UserName = userName
            };

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(Microsoft.AspNetCore.Mvc.Filters.ActionExecutedContext filterContext)
        {
            if (!(filterContext?.Controller?.ViewBag?.GaData is GaData))
            {
                base.OnActionExecuted(filterContext);
                return;
            }

            (Microsoft.AspNetCore.Mvc.Controller.ViewBag.GaData as GaData).LevyFlag 
                = (filterContext.Controller.ViewData.Model as OrchestratorResponse<AccountDashboardViewModel>)?.Data?.ApprenticeshipEmployerType.ToString();

            base.OnActionExecuted(filterContext);
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

            public string AgreementId { get; set; }
            public string LevyFlag { get; set; }

            public IDictionary<string, string> Extras { get; set; } = new Dictionary<string, string>();
        }
    }
}