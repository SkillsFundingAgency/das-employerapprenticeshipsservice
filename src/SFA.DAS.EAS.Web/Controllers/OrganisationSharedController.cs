using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers
{
    public class OrganisationSharedController : Controller
    {
        [HttpGet]
        [Route("accounts/{HashedAccountId}/organisations/custom/add", Order = 0)]
        [Route("accounts/organisations/custom/add", Order = 1)]
        public ActionResult AddCustomOrganisationDetails()
        {
            return Redirect(Url.EmployerProjectionsAction("accounts/{HashedAccountId}/organisations/custom/add"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("accounts/{HashedAccountId}/organisations/custom/add", Order = 0)]
        [Route("accounts/organisations/custom/add", Order = 1)]
        public async Task<ActionResult> AddOtherOrganisationDetails()
        {
            return Redirect(Url.EmployerAccountsAction("accounts/{HashedAccountId}/organisations/custom/add"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("accounts/{HashedAccountId}/organisations/address/select", Order = 0)]
        [Route("accounts/organisations/address/select", Order = 1)]
        public async Task<ActionResult> SelectAddress()
        {
            return Redirect(Url.EmployerAccountsAction("accounts/{HashedAccountId}/organisations/address/select"));
        }

        [HttpGet]
        [Route("accounts/{HashedAccountId}/organisations/address/update", Order = 0)]
        [Route("accounts/organisations/address/update", Order = 1)]
        public ActionResult AddOrganisationAddress()
        {
            return Redirect(Url.EmployerAccountsAction("accounts/{HashedAccountId}/organisations/address/update"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("accounts/{HashedAccountId}/organisations/address/update", Order = 0)]
        [Route("accounts/organisations/address/update", Order = 1)]
        public ActionResult UpdateOrganisationAddress()
        {
            return Redirect(Url.EmployerAccountsAction("accounts/{HashedAccountId}/organisations/address/update"));
        }
    }
}