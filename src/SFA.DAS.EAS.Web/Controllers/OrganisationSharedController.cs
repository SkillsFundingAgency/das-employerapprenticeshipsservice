using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.Controllers
{
    public class OrganisationSharedController : BaseController
    {
        private readonly OrganisationOrchestrator _orchestrator;
        private readonly IMapper _mapper;

        public OrganisationSharedController(IAuthenticationService owinWrapper,
            OrganisationOrchestrator orchestrator,
            IAuthorizationService authorization,
            IMultiVariantTestingService multiVariantTestingService,
            IMapper mapper,
            ILog logger,
            ICookieStorageService<FlashMessageViewModel> flashMessage)
            : base(owinWrapper, multiVariantTestingService, flashMessage)

        {
            _orchestrator = orchestrator;
            _mapper = mapper;
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
            return Redirect(Url.EmployerAccountsAction(("accounts/{HashedAccountId}/organisations/address/update")));
        }
    }
}