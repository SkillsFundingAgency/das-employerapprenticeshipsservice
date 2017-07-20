using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Organisation;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
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
        //This is temporary until the existing add org function is replaced, at which point the method used can be moved to the org search orchestrator
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
        public ActionResult SearchForOrganisation(string hashedAccountId)
        {
            var model = new OrchestratorResponse<SearchOrganisationViewModel> { Data = new SearchOrganisationViewModel { IsExistingAccount = !string.IsNullOrEmpty(hashedAccountId) } };
            return View("SearchForOrganisation", model);
        }

        [HttpPost]
        [Route("{HashedAccountId}/organisations/search", Order = 0)]
        [Route("organisations/search", Order = 1)]
        public ActionResult SearchForOrganisation(string hashedAccountId, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                var model = CreateSearchTermValidationErrorModel(new SearchOrganisationViewModel { IsExistingAccount = !string.IsNullOrEmpty(hashedAccountId)});
                return View("SearchForOrganisation", model);
            }

            return RedirectToAction("SearchForOrganisationResults", new { hashedAccountId, searchTerm });
        }

        [Route("{HashedAccountId}/organisations/search/results", Order = 0)]
        [Route("organisations/search/results", Order = 1)]
        public async Task<ActionResult> SearchForOrganisationResults(string hashedAccountId, string searchTerm, int pageNumber = 1, OrganisationType? organisationType = null)
        {
            OrchestratorResponse<SearchOrganisationResultsViewModel> model;
            if (string.IsNullOrEmpty(searchTerm))
            {
                var viewModel = new SearchOrganisationResultsViewModel { Results = new PagedResponse<OrganisationDetailsViewModel>() };
                model = CreateSearchTermValidationErrorModel(viewModel);
            }
            else
            {
                model = await _orchestrator.SearchOrganisation(searchTerm, pageNumber, organisationType, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
            }
            model.Data.IsExistingAccount = !string.IsNullOrEmpty(hashedAccountId);

            return View("SearchForOrganisationResults", model);
        }

        [HttpPost]
        [Route("{HashedAccountId}/organisations/search/confirm", Order = 0)]
        [Route("organisations/search/confirm", Order = 1)]
        public ActionResult Confirm(string hashedAccountId, OrganisationDetailsViewModel viewModel)
        {
            viewModel.NewSearch = true;

            if (string.IsNullOrWhiteSpace(viewModel.Address))
            {
                return FindAddress(hashedAccountId, viewModel);
            }
            CreateOrganisationCookieData(viewModel);

            if (string.IsNullOrEmpty(hashedAccountId))
            {
                return RedirectToAction("GatewayInform", "EmployerAccount");
            }

            
            var response = new OrchestratorResponse<OrganisationDetailsViewModel> { Data = viewModel };
            
            return View("../Organisation/ConfirmOrganisationDetails", response);
        }

        [HttpGet]
        [Route("{HashedAccountId}/organisations/search/manualAdd", Order = 0)]
        [Route("organisations/search/manualAdd", Order = 1)]
        public ActionResult AddOtherOrganisationDetails(string hashedAccountId)
        {
            if (string.IsNullOrEmpty(hashedAccountId))
            {
                return RedirectToAction("AddOtherOrganisationDetails", "EmployerAccountOrganisation");
            }

            return RedirectToAction("AddOtherOrganisationDetails", "Organisation");
        }

        private ActionResult FindAddress(string hashedAccountId, OrganisationDetailsViewModel organisation)
        {
            var addressViewModel = _mapper.Map<FindOrganisationAddressViewModel>(organisation);
            var response = new OrchestratorResponse<FindOrganisationAddressViewModel> { Data = addressViewModel };

            if (string.IsNullOrEmpty(hashedAccountId))
            {
                return View("../EmployerAccountOrganisation/FindAddress", response);
            }
            else
            {
                return View("../Organisation/FindAddress", response);
            }
        }

        private OrchestratorResponse<T> CreateSearchTermValidationErrorModel<T>(T data)
        {
            var model = new OrchestratorResponse<T> { Data = data };
            SetSearchTermValidationModelProperties(model);
            return model;
        }

        private OrchestratorResponse CreateSearchTermValidationErrorModel()
        {
            var model = new OrchestratorResponse();
            SetSearchTermValidationModelProperties(model);
            return model;
        }

        private static void SetSearchTermValidationModelProperties(OrchestratorResponse model)
        {
            model.Status = HttpStatusCode.BadRequest;
            model.FlashMessage = FlashMessageViewModel.CreateErrorFlashMessageViewModel(new Dictionary<string, string> { { "searchTerm", "Enter organisation name" } });
        }

        private void CreateOrganisationCookieData(OrganisationDetailsViewModel viewModel)
        {
            EmployerAccountData data;
            if (viewModel?.Name != null)
            {
                data = new EmployerAccountData
                {
                    OrganisationType = viewModel.Type,
                    OrganisationReferenceNumber = viewModel.ReferenceNumber,
                    OrganisationName = viewModel.Name,
                    OrganisationDateOfInception = viewModel.DateOfInception,
                    OrganisationRegisteredAddress = viewModel.Address,
                    OrganisationStatus = viewModel.Status ?? string.Empty,
                    PublicSectorDataSource = viewModel.PublicSectorDataSource,
                    Sector = viewModel.Sector,
                    NewSearch = viewModel.NewSearch
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
                    Sector = existingData.Sector,
                    NewSearch = existingData.NewSearch
                };
            }

            _orchestrator.CreateCookieData(HttpContext, data);
        }
    }
}