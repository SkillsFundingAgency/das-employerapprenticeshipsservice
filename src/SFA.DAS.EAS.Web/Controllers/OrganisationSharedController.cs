using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Authentication;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Web.ViewModels;

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

        /// <summary>
        /// AML-2459: Unused entry point
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("accounts/{HashedAccountId}/organisations/custom/add", Order = 0)]
        [Route("accounts/organisations/custom/add", Order = 1)]
        public ActionResult AddCustomOrganisationDetails()
        {
            return Redirect(Url.EmployerProjectionsAction("accounts/{HashedAccountId}/organisations/custom/add"));
        }

        /// <summary>
        /// AML-2459: Unused entry point
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("accounts/{HashedAccountId}/organisations/custom/add", Order = 0)]
        [Route("accounts/organisations/custom/add", Order = 1)]
        public async Task<ActionResult> AddOtherOrganisationDetails()
        {
            return Redirect(Url.EmployerAccountsAction("accounts/{HashedAccountId}/organisations/custom/add"));
        }

        /// <summary>
        /// AML-2459: Unused entry point
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("accounts/{HashedAccountId}/organisations/address/find", Order = 0)]
        [Route("accounts/organisations/address/find", Order = 1)]
        public ActionResult FindAddress()
        {
            return Redirect(Url.EmployerAccountsAction("accounts/{HashedAccountId}/organisations/address/find"));
        }

        /// <summary>
        /// AML-2459: Unused entry point
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("accounts/{HashedAccountId}/organisations/address/select", Order = 0)]
        [Route("accounts/organisations/address/select", Order = 1)]
        public async Task<ActionResult> SelectAddress()
        {
            return Redirect(Url.EmployerAccountsAction("accounts/{HashedAccountId}/organisations/address/select"));
        }

        /// <summary>
        /// AML-2459: Unused entry point
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("accounts/{HashedAccountId}/organisations/address/update", Order = 0)]
        [Route("accounts/organisations/address/update", Order = 1)]
        public ActionResult AddOrganisationAddress()
        {
            return Redirect(Url.EmployerAccountsAction("accounts/{HashedAccountId}/organisations/address/update"));
        }

        /// <summary>
        /// AML-2459: Unused entry point
        /// </summary>
        /// <returns></returns>
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