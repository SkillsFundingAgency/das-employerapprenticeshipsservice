using System.Collections.Generic;
using System.Web.Mvc;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Filters
{
    public class GoogleAnalyticsFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string userId = null;
            string hashedAccountId = null;
            string hashedLegalEntityId = null;

            var thisController = filterContext.Controller as BaseController;
            if (thisController != null)
                userId = thisController.OwinWrapper.GetClaimValue(@"sub");

            if (filterContext.ActionParameters.ContainsKey("hashedAccountId"))
            {
                hashedAccountId = filterContext.ActionParameters["hashedAccountId"] as string;
            }

            if (filterContext.ActionParameters.ContainsKey("hashedLegalEntityId"))
            {
                hashedLegalEntityId = filterContext.ActionParameters["hashedLegalEntityId"] as string;
            }

            

            filterContext.Controller.ViewBag.GaData = new GaData
            {
                UserId = userId,
                Acc = hashedAccountId,
                LegalEntityId = hashedLegalEntityId
            };

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!(filterContext?.Controller?.ViewBag?.GaData is GaData))
            {
                base.OnActionExecuted(filterContext);
                return;
            }

            (filterContext.Controller.ViewBag.GaData as GaData).LevyFlag 
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

            public string Vpv { get; set; }
            public string Acc { get; set; }

            public string LegalEntityId { get; set; }
            public string LevyFlag { get; set; }

            public IDictionary<string, string> Extras { get; set; } = new Dictionary<string, string>();
        }
    }
}