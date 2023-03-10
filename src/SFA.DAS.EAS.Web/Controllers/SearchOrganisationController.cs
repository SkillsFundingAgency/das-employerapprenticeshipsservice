using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Web.Controllers
{
    [DasAuthorize]
    [Route("accounts")]
    public class SearchOrganisationController : Controller
    {
        public EmployerApprenticeshipsServiceConfiguration Configuration { get; set; }
        public SearchOrganisationController(EmployerApprenticeshipsServiceConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        [HttpGet]
        [Route("{HashedAccountId}/organisations/search", Order = 0)]
        [Route("organisations/search", Order = 1)]
        public ActionResult SearchForOrganisation()
        {
            return Redirect(Url.EmployerAccountsAction("organisations/search", Configuration));
        }
        
        [Route("{HashedAccountId}/organisations/search/results", Order = 0)]
        [Route("organisations/search/results", Order = 1)]
        public async Task<ActionResult> SearchForOrganisationResults()
        {
            var paramString = Request?.Query == null ? string.Empty : $"?{Request.Query}";

            return Redirect(Url.EmployerAccountsAction($"organisations/search/results{paramString}", Configuration));
        }
    }
}