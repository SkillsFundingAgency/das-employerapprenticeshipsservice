using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [DasAuthorize]
    [RoutePrefix("accounts/{HashedAccountId}/organisations")]
    public class OrganisationController : BaseController
    {
        private readonly OrganisationOrchestrator _orchestrator;     
        private readonly IMapper _mapper;
        private readonly ILog _logger;

        public OrganisationController(
            IAuthenticationService owinWrapper,
            OrganisationOrchestrator orchestrator,
            IMultiVariantTestingService multiVariantTestingService,
            IMapper mapper,
            ILog logger,
            ICookieStorageService<FlashMessageViewModel> flashMessage)
            : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            _orchestrator = orchestrator;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Route("nextStep")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> OrganisationAddedNextSteps(string organisationName, string hashedAccountId, string hashedAgreementId)
        {
            var userId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);

            var viewModel = await _orchestrator.GetOrganisationAddedNextStepViewModel(organisationName, userId, hashedAccountId, hashedAgreementId);

            viewModel.FlashMessage = GetFlashMessageViewModelFromCookie();

            return View(viewModel);
        }

        [HttpGet]
        [Route("nextStepSearch")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> OrganisationAddedNextStepsSearch(string organisationName, string hashedAccountId, string hashedAgreementId)
        {
            var userId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);

            var viewModel = await _orchestrator.GetOrganisationAddedNextStepViewModel(organisationName, userId, hashedAccountId, hashedAgreementId);

            viewModel.FlashMessage = GetFlashMessageViewModelFromCookie();

            return View(ControllerConstants.OrganisationAddedNextStepsViewName, viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("confirm")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Confirm(
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
                ExternalUserId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName),
                LegalEntityStatus = string.IsNullOrWhiteSpace(legalEntityStatus) ? null : legalEntityStatus,
                Source = organisationType,
                PublicSectorDataSource = publicSectorDataSource,
                Sector = sector
            };

            var response = await _orchestrator.CreateLegalEntity(request);

            if (response.Status == HttpStatusCode.Unauthorized)
            {
                return View(response);
            }

            var flashMessage = new FlashMessageViewModel
            {
                HiddenFlashMessageInformation = "page-organisations-added",
                Headline = $"{response.Data.EmployerAgreement.LegalEntityName} has been added",
                Severity = FlashMessageSeverityLevel.Success
            };

            AddFlashMessageToCookie(flashMessage);

            return RedirectToAction(newSearch ? ControllerConstants.OrganisationAddedNextStepsSearchActionName : ControllerConstants.OrganisationAddedNextStepsActionName, new
            {
                hashedAccountId,
                organisationName = name,
                hashedAgreementId = response.Data.EmployerAgreement.HashedAgreementId
            });
        }

        [HttpPost]
        [Route("nextStep")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> GoToNextStep(string nextStep, string hashedAccountId, string organisationName, string hashedAgreementId)
        {
            var userId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);

            var userShownWizard = await _orchestrator.UserShownWizard(userId, hashedAccountId);

            switch (nextStep)
            {
                case "agreement": return RedirectToAction(ControllerConstants.AboutYourAgreementActionName, ControllerConstants.EmployerAgreementControllerName, new { agreementid = hashedAgreementId });

                case "teamMembers": return RedirectToAction(ControllerConstants.ViewTeamActionName, ControllerConstants.EmployerTeamControllerName, new { hashedAccountId });

                case "addOrganisation": return RedirectToAction(ControllerConstants.SearchForOrganisationActionName, ControllerConstants.SearchOrganisationControllerName, new { hashedAccountId });

                case "dashboard": return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerTeamControllerName, new { hashedAccountId });

                default:
                    var errorMessage = "Please select one of the next steps below";
                    return View(ControllerConstants.OrganisationAddedNextStepsViewName, new OrchestratorResponse<OrganisationAddedNextStepsViewModel>
                    {
                        Data = new OrganisationAddedNextStepsViewModel { ErrorMessage = errorMessage, OrganisationName = organisationName, ShowWizard = userShownWizard, HashedAgreementId = hashedAgreementId },
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

        [HttpGet]
        [Route("review")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Review(string hashedAccountId, string accountLegalEntityPublicHashedId)
        {
            var viewModel = await _orchestrator.GetRefreshedOrganisationDetails(accountLegalEntityPublicHashedId);

            if ((viewModel.Data.UpdatesAvailable & OrganisationUpdatesAvailable.Any) != 0)
            {
                return View(viewModel);
            }

            return View("ReviewNoChange", viewModel);
        }

        [HttpPost]
        [Route("review")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> ProcessReviewSelection(
            string updateChoice,
            string hashedAccountId,
            string accountLegalEntityPublicHashedId,
            string organisationName,
            string organisationAddress,
            string dataSourceFriendlyName)
        {
            switch (updateChoice)
            {
                case "update":

                    var userId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);

                    var response = await _orchestrator.UpdateOrganisation(
                        accountLegalEntityPublicHashedId,
                        organisationName,
                        organisationAddress, hashedAccountId, userId);

                    return View(ControllerConstants.OrganisationUpdatedNextStepsActionName, response);

                case "incorrectDetails":
                    return View("ReviewIncorrectDetails", new IncorrectOrganisationDetailsViewModel { DataSourceFriendlyName = dataSourceFriendlyName });
            }

            return RedirectToAction("Details", "EmployerAgreement");
        }

        [HttpPost]
        [Route("PostUpdateSelection")]
        public Microsoft.AspNetCore.Mvc.ActionResult GoToPostUpdateSelection(string nextStep, string hashedAccountId)
        {
            switch (nextStep)
            {
                case "dashboard":
                    return RedirectToAction("Index", "EmployerAgreement");

                case "homepage":
                    return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.EmployerTeamControllerName, new { HashedAccountId = hashedAccountId });   

                default:
                    var errorMessage = "Please select one of the next steps below";

                    return View(ControllerConstants.OrganisationUpdatedNextStepsActionName,
                        new OrchestratorResponse<OrganisationUpdatedNextStepsViewModel>
                        {
                            Data = new OrganisationUpdatedNextStepsViewModel { ErrorMessage = errorMessage, HashedAccountId = hashedAccountId },
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