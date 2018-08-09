using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts")]
    public class SearchOrganisationController : Controller
    {
        [HttpGet]
        [Route("{HashedAccountId}/organisations/search", Order = 0)]
        [Route("organisations/search", Order = 1)]
        public ActionResult SearchForOrganisation(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("organisations/search"));
        }

        [Route("{HashedAccountId}/organisations/search/results", Order = 0)]
        [Route("organisations/search/results", Order = 1)]
        public async Task<ActionResult> SearchForOrganisationResults(
            string hashedAccountId, string searchTerm, int pageNumber = 1,
            OrganisationType? organisationType = null)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.LegacyEasAccountAction($"organisations/search/results{paramString}"));
        }

        [HttpGet]
        [Route("{HashedAccountId}/organisations/search/manualAdd", Order = 0)]
        [Route("organisations/search/manualAdd", Order = 1)]
        public ActionResult AddOtherOrganisationDetails(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("organisations/search/manualAdd"));
        }
    }
}