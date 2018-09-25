using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Queries.GetActivities;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using System.Web.Mvc;
using SFA.DAS.EmployerAccounts.Queries.GetLatestActivities;
using SFA.DAS.EmployerAccounts.Web.Helpers;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}/activity")]
    public class ActivitiesController : Controller
    {
        [Route]
        public ActionResult Index(GetActivitiesQuery query)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.LegacyEasAccountAction($"activity{paramString}"));
        }

        [ChildActionOnly]
        public ActionResult Latest(GetLatestActivitiesQuery query)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.LegacyEasAccountAction($"activity/latest/{paramString}"));
        }
    }
}