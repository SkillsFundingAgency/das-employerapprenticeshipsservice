using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers
{
    [DasAuthorize]
    [RoutePrefix("accounts")]
    public class SearchOrganisationController : Controller
    {
        [HttpGet]
        [Route("{HashedAccountId}/organisations/search", Order = 0)]
        [Route("organisations/search", Order = 1)]
        public ActionResult SearchForOrganisation()
        {
            return Redirect(Url.EmployerAccountsAction("organisations/search"));
        }
        
        [Route("{HashedAccountId}/organisations/search/results", Order = 0)]
        [Route("organisations/search/results", Order = 1)]
        public async Task<ActionResult> SearchForOrganisationResults()
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.EmployerAccountsAction($"organisations/search/results{paramString}"));
        }
    }
}