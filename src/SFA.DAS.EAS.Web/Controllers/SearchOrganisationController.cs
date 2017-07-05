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
        private readonly OrganisationOrchestrator _organisationOrchestrator;
        private readonly IMapper _mapper;


        public SearchOrganisationController(IOwinWrapper owinWrapper,
            SearchOrganisationOrchestrator orchestrator,
            IFeatureToggle featureToggle,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage,
            IMapper mapper,
            OrganisationOrchestrator organisationOrchestrator)
            : base(owinWrapper, featureToggle, multiVariantTestingService, flashMessage)
        {
            _orchestrator = orchestrator;
            _mapper = mapper;
            _organisationOrchestrator = organisationOrchestrator;
        }

        [HttpGet]
        [Route("{HashedAccountId}/organisations/search", Order = 0)]
        [Route("organisations/search", Order = 1)]
        public ActionResult SearchForOrganisation()
        {
            return View("SearchForOrganisation");
        }

        [HttpPost]
        [Route("{HashedAccountId}/organisations/search", Order = 0)]
        [Route("organisations/search", Order = 1)]
        public ActionResult SearchForOrganisation(string hashedAccountId, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                var model = CreateSearchTermValidationErrorModel();
                return View("SearchForOrganisation", model);
            }

            return RedirectToAction("SearchForOrganisationResults", new { hashedAccountId, searchTerm });
        }

        [Route("{HashedAccountId}/organisations/search/results", Order = 0)]
        [Route("organisations/search/results", Order = 1)]
        public async Task<ActionResult> SearchForOrganisationResults(string hashedAccountId, string searchTerm, int pageNumber = 1, OrganisationType? organisationType = null)
        {
            OrchestratorResponse<SearchOrganisationViewModel> model;
            if (string.IsNullOrEmpty(searchTerm))
            {
                var viewModel = new SearchOrganisationViewModel { Results = new PagedResponse<OrganisationDetailsViewModel>() };
                model = CreateSearchTermValidationErrorModel(viewModel);
            }
            else
            {
                model = await _orchestrator.SearchOrganisation(searchTerm, pageNumber, organisationType, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
            }

            return View("SearchForOrganisationResults", model);
        }

        [HttpPost]
        [Route("{HashedAccountId}/organisations/search/confirm", Order = 0)]
        [Route("organisations/search/confirm", Order = 1)]
        public ActionResult Confirm(OrganisationDetailsViewModel viewModel)
        {
            CreateOrganisationCookieData(viewModel);
            return View(new ConfirmOrganisationViewModel { Name = viewModel.Name, Address = viewModel.Address });
        }

        [HttpGet]
        [Route("{HashedAccountId}/organisations/search/confirm", Order = 0)]
        [Route("organisations/search/confirm", Order = 1)]
        public async Task<ActionResult> Confirm(string hashedAccountId)
        {
            var organisation = _orchestrator.GetCookieData(HttpContext);

            if (string.IsNullOrWhiteSpace(organisation.OrganisationRegisteredAddress))
            {
                return FindAddress(CreateOrganisationDetailsViewModel(organisation));
            }

            if (string.IsNullOrEmpty(hashedAccountId))
            {
                return RedirectToAction("GatewayInform", "EmployerAccount");
            }

            return await CreateOrganisation(hashedAccountId, organisation);
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

        private async Task<ActionResult> CreateOrganisation(string hashedAccountId, EmployerAccountData organisation)
        {
            var request = new CreateNewLegalEntityViewModel
            {
                HashedAccountId = hashedAccountId,
                Name = organisation.OrganisationName,
                Code = organisation.OrganisationReferenceNumber,
                Address = organisation.OrganisationRegisteredAddress,
                IncorporatedDate = organisation.OrganisationDateOfInception,
                ExternalUserId = OwinWrapper.GetClaimValue(@"sub"),
                LegalEntityStatus = null,
                Source = (short)organisation.OrganisationType,
                PublicSectorDataSource = organisation.PublicSectorDataSource,
                Sector = organisation.Sector
            };

            var response = await _organisationOrchestrator.CreateLegalEntity(request);

            var flashMessage = new FlashMessageViewModel
            {
                HiddenFlashMessageInformation = "page-organisations-added",
                Headline = $"{response.Data.EmployerAgreement.LegalEntityName} has been added",
                Severity = FlashMessageSeverityLevel.Success
            };
            AddFlashMessageToCookie(flashMessage);

            return RedirectToAction("OrganisationAddedNextSteps", "Organisation", new { hashedAccountId });
        }

        private ActionResult FindAddress(OrganisationDetailsViewModel organisation)
        {
            var addressViewModel = _mapper.Map<FindOrganisationAddressViewModel>(organisation);

            var addressResponse = new OrchestratorResponse<FindOrganisationAddressViewModel>
            {
                Data = addressViewModel
            };

            return View("../EmployerAccountOrganisation/FindAddress", addressResponse);
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
            model.FlashMessage = FlashMessageViewModel.CreateErrorFlashMessageViewModel(new Dictionary<string, string> { { "searchTerm", "Enter organisation name/number" } });
        }

        private void CreateOrganisationCookieData(OrganisationDetailsViewModel viewModel)
        {
            EmployerAccountData data;
            if (viewModel?.Name != null)
            {
                data = new EmployerAccountData
                {
                    OrganisationType = viewModel.Type,
                    OrganisationReferenceNumber = viewModel.OrganisationCode,
                    OrganisationName = viewModel.Name,
                    OrganisationDateOfInception = viewModel.DateOfInception,
                    OrganisationRegisteredAddress = viewModel.Address,
                    OrganisationStatus = viewModel.Status ?? string.Empty,
                    PublicSectorDataSource = viewModel.PublicSectorDataSource,
                    Sector = viewModel.Sector
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

        private OrganisationDetailsViewModel CreateOrganisationDetailsViewModel(EmployerAccountData accountData)
        {
            return new OrganisationDetailsViewModel
            {
                Type = accountData.OrganisationType,
                OrganisationCode = accountData.OrganisationReferenceNumber,
                Name = accountData.OrganisationName,
                DateOfInception = accountData.OrganisationDateOfInception,
                Address = accountData.OrganisationRegisteredAddress,
                Status = accountData.OrganisationStatus,
                PublicSectorDataSource = accountData.PublicSectorDataSource,
                Sector = accountData.Sector
            };
        }
    }
}