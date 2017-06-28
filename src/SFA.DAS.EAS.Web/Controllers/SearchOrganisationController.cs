using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Organisation;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts")]
    public class SearchOrganisationController : BaseController
    {
        private readonly SearchOrganisationOrchestrator _orchestrator;
        private readonly IMapper _mapper;


        public SearchOrganisationController(IOwinWrapper owinWrapper,
            SearchOrganisationOrchestrator orchestrator,
            IFeatureToggle featureToggle,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage,
            IMapper mapper)
            : base(owinWrapper, featureToggle, multiVariantTestingService, flashMessage)
        {
            _orchestrator = orchestrator;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("{HashedAccountId}/organisations/search", Order = 0)]
        [Route("organisations/search", Order = 1)]
        public ActionResult SearchForOrganisation()
        {
            return View("SearchForOrganisation");
        }

        [Route("{HashedAccountId}/organisations/search/results", Order = 0)]
        [Route("organisations/search/results", Order = 1)]
        public async Task<ActionResult> SearchForOrganisationResults(string hashedAccountId, string searchTerm, int pageNumber = 1, OrganisationType? organisationType = null)
        {
            var model = await _orchestrator.SearchOrganisation(searchTerm, pageNumber, organisationType, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            return View("SearchForOrganisationResults", model);
        }
        
        [HttpPost]
        [Route("{HashedAccountId}/organisations/select", Order = 0)]
        [Route("organisations/select", Order = 1)]
        public ActionResult SelectOrganisation(OrganisationDetailsViewModel viewModel)
        {
            if (string.IsNullOrWhiteSpace(viewModel.Address))
            {
                var addressViewModel = _mapper.Map<FindOrganisationAddressViewModel>(viewModel);

                var addressResponse = new OrchestratorResponse<FindOrganisationAddressViewModel>
                {
                    Data = addressViewModel
                };
                
                return View("../EmployerAccountOrganisation/FindAddress", addressResponse);
            }

            var response = new OrchestratorResponse<OrganisationDetailsViewModel>
            {
                Data = viewModel,
                Status = HttpStatusCode.OK
            };

            CreateOrganisationCookieData(response);

            return RedirectToAction("GatewayInform", "EmployerAccount");
        }

        [HttpGet]
        [Route("{HashedAccountId}/organisations/confirm", Order = 0)]
        [Route("organisations/confirm", Order = 1)]
        public ActionResult Confirm()
        {
            var organisation = _orchestrator.GetCookieData(HttpContext);
            var viewModel = new ConfirmOrganisationViewModel { Name = organisation.OrganisationName, Address = organisation.OrganisationRegisteredAddress };
            return View(viewModel);
        }

        private void CreateOrganisationCookieData(OrchestratorResponse<OrganisationDetailsViewModel> response)
        {
            EmployerAccountData data;
            if (response.Data?.Name != null)
            {
                data = new EmployerAccountData
                {
                    OrganisationType = response.Data.Type,
                    OrganisationReferenceNumber = response.Data.ReferenceNumber,
                    OrganisationName = response.Data.Name,
                    OrganisationDateOfInception = response.Data.DateOfInception,
                    OrganisationRegisteredAddress = response.Data.Address,
                    OrganisationStatus = response.Data.Status ?? string.Empty,
                    PublicSectorDataSource = response.Data.PublicSectorDataSource,
                    Sector = response.Data.Sector
                };
            }
            else
            {
                var existingData = _orchestrator.GetCookieData(HttpContext);

                data = new EmployerAccountData
                {
                    OrganisationType = existingData.OrganisationType,
                    OrganisationReferenceNumber = existingData.OrganisationReferenceNumber,
                    OrganisationName = existingData.OrganisationName,
                    OrganisationDateOfInception = existingData.OrganisationDateOfInception,
                    OrganisationRegisteredAddress = existingData.OrganisationRegisteredAddress,
                    OrganisationStatus = existingData.OrganisationStatus,
                    PublicSectorDataSource = existingData.PublicSectorDataSource,
                    Sector = existingData.Sector
                };
            }

            _orchestrator.CreateCookieData(HttpContext, data);
        }
    }
}