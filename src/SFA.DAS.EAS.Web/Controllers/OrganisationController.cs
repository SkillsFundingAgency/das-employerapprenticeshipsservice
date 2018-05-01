using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}/organisations")]
    public class OrganisationController : BaseController
    {
        private readonly OrganisationOrchestrator _orchestrator;
        private readonly IMapper _mapper;
        private readonly ILog _logger;

        public OrganisationController(
            IAuthenticationService owinWrapper, 
            OrganisationOrchestrator orchestrator,
            IAuthorizationService authorization,
            IMultiVariantTestingService multiVariantTestingService,
            IMapper mapper,
            ILog logger,
            ICookieStorageService<FlashMessageViewModel> flashMessage) 
            :base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            _orchestrator = orchestrator;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("legalAgreement")]
        public ActionResult OrganisationLegalAgreement(string hashedAccountId, OrganisationDetailsViewModel model)
        {
            var viewModel = new OrchestratorResponse<OrganisationDetailsViewModel>
            {
                Data = model,
                Status = HttpStatusCode.OK
            };

            return View(viewModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("confirm")]
        public async Task<ActionResult> Confirm(
            string hashedAccountId, string name, string code, string address, DateTime? incorporated,
            string legalEntityStatus, OrganisationType organisationType, byte? publicSectorDataSource, string sector, bool newSearch)
        {
            var request = new CreateNewLegalEntityViewModel
            {
                HashedAccountId = hashedAccountId,
                Name = name,
                Code = code,
                Address = address,
                IncorporatedDate = incorporated,
                ExternalUserId = OwinWrapper.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName),
                LegalEntityStatus = string.IsNullOrWhiteSpace(legalEntityStatus) ? null : legalEntityStatus,
                Source = (byte)organisationType,
                PublicSectorDataSource = publicSectorDataSource,
                Sector = sector
            };

            var response = await _orchestrator.CreateLegalEntity(request);
            
            var flashMessage = new FlashMessageViewModel
            {
                HiddenFlashMessageInformation = "page-organisations-added",
                Headline = $"{response.Data.EmployerAgreement.LegalEntityName} has been added",
                Severity = FlashMessageSeverityLevel.Success
            };
            AddFlashMessageToCookie(flashMessage);
            if (newSearch)
            {
                return RedirectToAction(ControllerConstants.OrganisationAddedNextStepsSearchActionName, new { hashedAccountId, organisationName = name, hashedAgreementId = response.Data.EmployerAgreement.HashedAgreementId });
            }
            return RedirectToAction(ControllerConstants.OrganisationAddedNextStepsActionName, new { hashedAccountId, organisationName = name });
        }

        [HttpGet]
        [Route("nextStep")]
        public async Task<ActionResult> OrganisationAddedNextSteps(string organisationName, string hashedAccountId)
        {
            var userId = OwinWrapper.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName);

            var viewModel = await _orchestrator.GetOrganisationAddedNextStepViewModel(organisationName, userId, hashedAccountId);

            viewModel.FlashMessage = GetFlashMessageViewModelFromCookie();

            return View(viewModel);
        }

        [HttpGet]
        [Route("nextStepSearch")]
        public async Task<ActionResult> OrganisationAddedNextStepsSearch(string organisationName, string hashedAccountId, string hashedAgreementId)
        {
            var userId = OwinWrapper.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName);

            var viewModel = await _orchestrator.GetOrganisationAddedNextStepViewModel(organisationName, userId, hashedAccountId, hashedAgreementId);

            viewModel.FlashMessage = GetFlashMessageViewModelFromCookie();

            return View(ControllerConstants.OrganisationAddedNextStepsViewName, viewModel);
        }


        [HttpPost]
        [Route("nextStep")]
        public async Task<ActionResult> GoToNextStep(string nextStep, string hashedAccountId, string organisationName, string hashedAgreementId)
        {
            var userId = OwinWrapper.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName);

            var userShownWizard = await _orchestrator.UserShownWizard(userId, hashedAccountId);

            switch (nextStep)
            {
                case "agreement": return RedirectToAction(ControllerConstants.AboutYourAgreement, ControllerConstants.EmployerAgreementControllerName, new { agreementid = hashedAgreementId });

                case "teamMembers": return RedirectToAction(ControllerConstants.ViewTeamActionName, ControllerConstants.EmployerTeamControllerName, new { hashedAccountId });

                case "addOrganisation": return RedirectToAction(ControllerConstants.SearchForOrganisationActionName, ControllerConstants.SearchOrganisationControllerName, new { hashedAccountId });

                case "dashboard": return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerTeamControllerName, new { hashedAccountId });

                default:
                    var errorMessage = "Please select one of the next steps below";
                    return View(ControllerConstants.OrganisationAddedNextStepsViewName, new OrchestratorResponse<OrganisationAddedNextStepsViewModel>
                    {
                        Data = new OrganisationAddedNextStepsViewModel { ErrorMessage = errorMessage, OrganisationName = organisationName, ShowWizard = userShownWizard, HashedAgreementId = hashedAgreementId},
                        FlashMessage = new FlashMessageViewModel
                        {
                            Headline = "Invalid next step chosen",
                            Message = errorMessage,
                            ErrorMessages = new Dictionary<string, string> { { "nextStep", errorMessage } },
                            Severity = FlashMessageSeverityLevel.Error
                        }
                    });
            }
        }
    }
}