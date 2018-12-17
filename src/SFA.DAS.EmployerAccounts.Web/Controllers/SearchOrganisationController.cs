using System.Collections.Generic;
using System.Net;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.ReferenceData;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts")]
    public class SearchOrganisationController : BaseController
    {
        private readonly SearchOrganisationOrchestrator _orchestrator;
        //This is temporary until the existing add org function is replaced, at which point the method used can be moved to the org search orchestrator
        private readonly IMapper _mapper;


        public SearchOrganisationController(IAuthenticationService owinWrapper,
            SearchOrganisationOrchestrator orchestrator,
            IAuthorizationService authorization,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage,
            IMapper mapper)
            : base(owinWrapper, multiVariantTestingService, flashMessage)
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
            return View(ControllerConstants.SearchForOrganisationViewName, model);
        }

        [HttpPost]
        [Route("{HashedAccountId}/organisations/search", Order = 0)]
        [Route("organisations/search", Order = 1)]
        public ActionResult SearchForOrganisation(string hashedAccountId, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                var model = CreateSearchTermValidationErrorModel(new SearchOrganisationViewModel { IsExistingAccount = !string.IsNullOrEmpty(hashedAccountId) });
                return View(ControllerConstants.SearchForOrganisationViewName, model);
            }

            return RedirectToAction(ControllerConstants.SearchForOrganisationResultsActionName, new { hashedAccountId, searchTerm });
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

            return View(ControllerConstants.SearchForOrganisationResultsViewName, model);
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
            viewModel.CreateOrganisationCookie(_orchestrator, HttpContext);

            if (string.IsNullOrEmpty(hashedAccountId))
            {
                return RedirectToAction(ControllerConstants.GatewayInformActionName, ControllerConstants.EmployerAccountControllerName);
            }


            var response = new OrchestratorResponse<OrganisationDetailsViewModel> { Data = viewModel };

            return View(ControllerConstants.ConfirmOrganisationDetailsViewName, response);
        }

        [HttpGet]
        [Route("{HashedAccountId}/organisations/search/manualAdd", Order = 0)]
        [Route("organisations/search/manualAdd", Order = 1)]
        public ActionResult AddOtherOrganisationDetails(string hashedAccountId)
        {
            return RedirectToAction(ControllerConstants.AddOtherOrganisationDetailsViewName, ControllerConstants.OrganisationSharedControllerName);
        }

        private ActionResult FindAddress(string hashedAccountId, OrganisationDetailsViewModel organisation)
        {
            var addressViewModel = _mapper.Map<FindOrganisationAddressViewModel>(organisation);
            var response = new OrchestratorResponse<FindOrganisationAddressViewModel> { Data = addressViewModel };
            return View(ControllerConstants.FindAddressViewName, response);
        }

        private OrchestratorResponse<T> CreateSearchTermValidationErrorModel<T>(T data)
        {
            var model = new OrchestratorResponse<T> { Data = data };
            SetSearchTermValidationModelProperties(model);
            return model;
        }

        private static void SetSearchTermValidationModelProperties(OrchestratorResponse model)
        {
            model.Status = HttpStatusCode.BadRequest;
            model.FlashMessage = FlashMessageViewModel.CreateErrorFlashMessageViewModel(new Dictionary<string, string> { { "searchTerm", "Enter organisation name" } });
        }
    }
}