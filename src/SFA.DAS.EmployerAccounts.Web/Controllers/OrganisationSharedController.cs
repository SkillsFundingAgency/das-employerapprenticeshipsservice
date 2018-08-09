using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    public class OrganisationSharedController : Controller
    {
        [HttpGet]
        [Route("accounts/{HashedAccountId}/organisations/custom/add", Order = 0)]
        [Route("accounts/organisations/custom/add", Order = 1)]
        public ActionResult AddCustomOrganisationDetails(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("organisations/custom/add"));
        }

        [HttpGet]
        [Route("accounts/{HashedAccountId}/organisations/address/update", Order = 0)]
        [Route("accounts/organisations/address/update", Order = 1)]
        public ActionResult AddOrganisationAddress(AddOrganisationAddressViewModel request)
        {
            var paramString = Request?.Url?.Query == null ? string.Empty : $"?{Request.Url.Query}";

            return Redirect(Url.LegacyEasAccountAction($"organisations/address/update{paramString}"));
        }
    }
}