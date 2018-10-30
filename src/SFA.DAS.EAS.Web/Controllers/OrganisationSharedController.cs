using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Authentication;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;

namespace SFA.DAS.EAS.Web.Controllers
{
    public class OrganisationSharedController : BaseController
    {
        public OrganisationSharedController(IAuthenticationService owinWrapper,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage)
            : base(owinWrapper, multiVariantTestingService, flashMessage)

        {
        }

        [HttpGet]
        [Route("accounts/{HashedAccountId}/organisations/custom/add", Order = 0)]
        [Route("accounts/organisations/custom/add", Order = 1)]
        public ActionResult AddCustomOrganisationDetails(string hashedAccountId)
        {
            return Redirect(Url.EmployerProjectionsAction("accounts/{HashedAccountId}/organisations/custom/add"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("accounts/{HashedAccountId}/organisations/custom/add", Order = 0)]
        [Route("accounts/organisations/custom/add", Order = 1)]
        public async Task<ActionResult> AddOtherOrganisationDetails(OrganisationDetailsViewModel model)
        {
            return Redirect(Url.EmployerAccountsAction("accounts/{HashedAccountId}/organisations/custom/add"));
        }

        [HttpPost]
        [Route("accounts/{HashedAccountId}/organisations/address/find", Order = 0)]
        [Route("accounts/organisations/address/find", Order = 1)]
        public ActionResult FindAddress(FindOrganisationAddressViewModel request)
        {
            return Redirect(Url.EmployerAccountsAction("accounts/{HashedAccountId}/organisations/address/find"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("accounts/{HashedAccountId}/organisations/address/select", Order = 0)]
        [Route("accounts/organisations/address/select", Order = 1)]
        public async Task<ActionResult> SelectAddress(FindOrganisationAddressViewModel request)
        {
            return Redirect(Url.EmployerAccountsAction("accounts/{HashedAccountId}/organisations/address/select"));
        }

        [HttpGet]
        [Route("accounts/{HashedAccountId}/organisations/address/update", Order = 0)]
        [Route("accounts/organisations/address/update", Order = 1)]
        public ActionResult AddOrganisationAddress(AddOrganisationAddressViewModel request)
        {
            return Redirect(Url.EmployerAccountsAction("accounts/{HashedAccountId}/organisations/address/update"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("accounts/{HashedAccountId}/organisations/address/update", Order = 0)]
        [Route("accounts/organisations/address/update", Order = 1)]
        public ActionResult UpdateOrganisationAddress(AddOrganisationAddressViewModel request)
        {
            return Redirect(Url.EmployerAccountsAction("accounts/{HashedAccountId}/organisations/address/update"));
        }
    }
}