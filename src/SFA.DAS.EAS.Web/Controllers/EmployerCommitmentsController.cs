using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Enums;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.Exceptions;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SFA.DAS.EAS.Web.Validators;
using FluentValidation.Mvc;
using FluentValidation;

namespace SFA.DAS.EAS.Web.Controllers
{

    [Authorize]
    [RoutePrefix("accounts/{hashedaccountId}/apprentices")]
    public class EmployerCommitmentsController : BaseController
    {
        private readonly EmployerCommitmentsOrchestrator _employerCommitmentsOrchestrator;

        private const string LastCohortPageSessionKey = "lastCohortPageSessionKey";

        public EmployerCommitmentsController(EmployerCommitmentsOrchestrator employerCommitmentsOrchestrator, IOwinWrapper owinWrapper,
            IFeatureToggle featureToggle, IUserWhiteList userWhiteList)
            : base(owinWrapper, featureToggle)
        {
            if (employerCommitmentsOrchestrator == null)
                throw new ArgumentNullException(nameof(employerCommitmentsOrchestrator));
            if (owinWrapper == null)
                throw new ArgumentNullException(nameof(owinWrapper));

            _employerCommitmentsOrchestrator = employerCommitmentsOrchestrator;
        }

        [HttpGet]
        [Route("home")]
        public async Task<ActionResult> Index(string hashedAccountId)
        {
            ViewBag.HashedAccountId = hashedAccountId;

            var response = await _employerCommitmentsOrchestrator.CheckAccountAuthorization(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            return View(response);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("cohorts")]
        public async Task<ActionResult> YourCohorts(string hashedAccountId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await _employerCommitmentsOrchestrator.GetYourCohorts(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            SetFlashMessageOnModel(model);
            return View(model);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("cohorts/new")]
        public async Task<ActionResult> WaitingToBeSent(string hashedAccountId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await _employerCommitmentsOrchestrator.GetAllWaitingToBeSent(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
            SetFlashMessageOnModel(model);
            Session[LastCohortPageSessionKey] = RequestStatus.NewRequest;
            return View("RequestList", model);
        }

        [HttpGet]
        [Route("cohorts/approve")]
        public async Task<ActionResult> ReadyForApproval(string hashedAccountId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await _employerCommitmentsOrchestrator.GetAllReadyForApproval(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
            SetFlashMessageOnModel(model);
            Session[LastCohortPageSessionKey] = RequestStatus.ReadyForApproval;
            return View("RequestList", model);
        }

        [HttpGet]
        [Route("cohorts/review")]
        public async Task<ActionResult> ReadyForReview(string hashedAccountId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor ))
                return View("AccessDenied");

            var model = await _employerCommitmentsOrchestrator.GetAllReadyForReview(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
            SetFlashMessageOnModel(model);
            Session[LastCohortPageSessionKey] = RequestStatus.ReadyForReview;
            return View("RequestList", model);
        }

        [HttpGet]
        [Route("cohorts/provider")]
        public async Task<ActionResult> WithProvider(string hashedAccountId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await _employerCommitmentsOrchestrator.GetAllWithProvider(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
            return View("RequestList", model);
        }

        [HttpGet]
        [Route("Inform")]
        public async Task<ActionResult> Inform(string hashedAccountId)
        {
            var response = await _employerCommitmentsOrchestrator.GetInform(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            return View(response);
        }

        [HttpGet]
        [Route("legalEntity/create")]
        public async Task<ActionResult> SelectLegalEntity(string hashedAccountId, string cohortRef = "")
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var response = await _employerCommitmentsOrchestrator
                .GetLegalEntities(hashedAccountId, cohortRef, OwinWrapper.GetClaimValue(@"sub"));

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("legalEntity/create")]
        public async Task<ActionResult> SetLegalEntity(string hashedAccountId, SelectLegalEntityViewModel selectedLegalEntity)
        {
            if (!ModelState.IsValid)
            {
                var response = await _employerCommitmentsOrchestrator.GetLegalEntities(hashedAccountId, selectedLegalEntity.CohortRef, OwinWrapper.GetClaimValue(@"sub"));

                return View("SelectLegalEntity", response);
            }

            var agreement = await _employerCommitmentsOrchestrator.GetLegalEntitySignedAgreementViewModel(hashedAccountId,
                selectedLegalEntity.LegalEntityCode, selectedLegalEntity.CohortRef);

            if (agreement.Data.HasSignedAgreement)
            {
                return RedirectToAction("SearchProvider", selectedLegalEntity);
            }
            else
            {
                return RedirectToAction("AgreementNotSigned", agreement.Data);
            }
            
        }

        [HttpGet]
        [Route("provider/create")]
        public async Task<ActionResult> SearchProvider(string hashedAccountId, string legalEntityCode, string cohortRef)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            if (string.IsNullOrWhiteSpace(legalEntityCode) || string.IsNullOrWhiteSpace(cohortRef))
            {
                return RedirectToAction("Inform", new {hashedAccountId});
            }

            var response = await _employerCommitmentsOrchestrator.GetProviderSearch(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), legalEntityCode, cohortRef);

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("provider/create")]
        public async Task<ActionResult> SelectProvider(string hashedAccountId, [System.Web.Http.FromUri] [CustomizeValidator(RuleSet = "Request")] SelectProviderViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var defaultViewModel = await _employerCommitmentsOrchestrator.GetProviderSearch(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), viewModel.LegalEntityCode, viewModel.CohortRef);
                
                return View("SearchProvider", defaultViewModel);
            }

            var response = await _employerCommitmentsOrchestrator.GetProvider(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), viewModel);

            if (response.Data.Provider == null)
            {
                var defaultViewModel = await _employerCommitmentsOrchestrator.GetProviderSearch(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), viewModel.LegalEntityCode, viewModel.CohortRef);
                defaultViewModel.Data.NotFound = true;

                RevalidateModel(defaultViewModel);

                return View("SearchProvider", defaultViewModel);
            }

            return View(response);
        }

        private void RevalidateModel(OrchestratorResponse<SelectProviderViewModel> defaultViewModel)
        {
            var validator = new SelectProviderViewModelValidator();
            var results = validator.Validate(defaultViewModel.Data, ruleSet: "SearchResult");
            
            results.AddToModelState(ModelState, null);
        }

        [HttpGet]
        [Route("confirmProvider/create")]
        public async Task<ActionResult> ConfirmProvider(string hashedAccountId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            return RedirectToAction("Inform", new { hashedAccountId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("confirmProvider/create")]
        public async Task<ActionResult> ConfirmProvider(string hashedAccountId, [System.Web.Http.FromUri]ConfirmProviderViewModel viewModelModel)
        {
            if (!ModelState.IsValid)
            {
                if (viewModelModel.Confirmation == null)
                {
                    var response = await _employerCommitmentsOrchestrator.GetProvider(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), viewModelModel);

                    return View("SelectProvider", response);
                }
            }

            if (!viewModelModel.Confirmation.Value)
            {
                return RedirectToAction("SearchProvider", new SelectProviderViewModel { LegalEntityCode = viewModelModel.LegalEntityCode, CohortRef = viewModelModel.CohortRef });
            }

            return RedirectToAction("ChoosePath", new { hashedAccountId = hashedAccountId, legalEntityCode = viewModelModel.LegalEntityCode, providerId = viewModelModel.ProviderId, cohortRef = viewModelModel.CohortRef });
        }

        [HttpGet]
        [Route("choosePath/create")]
        public async Task<ActionResult> ChoosePath(string hashedAccountId, string legalEntityCode, string providerId, string cohortRef)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            if (string.IsNullOrWhiteSpace(legalEntityCode)
                || string.IsNullOrWhiteSpace(providerId)
                || string.IsNullOrWhiteSpace(cohortRef))
            {
                return RedirectToAction("Inform", new { hashedAccountId });
            }

            var model = await _employerCommitmentsOrchestrator.CreateSummary(hashedAccountId, legalEntityCode, providerId, cohortRef, OwinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("choosePath/create")]
        public async Task<ActionResult> CreateCommitment(CreateCommitmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var model = await _employerCommitmentsOrchestrator.CreateSummary(viewModel.HashedAccountId, viewModel.LegalEntityCode, viewModel.ProviderId.ToString(), viewModel.CohortRef,  OwinWrapper.GetClaimValue(@"sub"));

                return View("ChoosePath", model);
            }

            if (viewModel.SelectedRoute == "employer")
            {
                var userDisplayName = OwinWrapper.GetClaimValue(DasClaimTypes.DisplayName);
                var userEmail = OwinWrapper.GetClaimValue(DasClaimTypes.Email);
                var userId = OwinWrapper.GetClaimValue(@"sub");

                var response = await _employerCommitmentsOrchestrator.CreateEmployerAssignedCommitment(viewModel, userId, userDisplayName, userEmail);

                return RedirectToAction("Details", new { hashedCommitmentId = response.Data });
            }

            return RedirectToAction("SubmitNewCommitment",
                new { hashedAccountId = viewModel.HashedAccountId, legalEntityCode = viewModel.LegalEntityCode, legalEntityName = viewModel.LegalEntityName, legalEntityAddress = viewModel.LegalEntityAddress, legalEntitySource = viewModel.LegalEntitySource, providerId = viewModel.ProviderId, providerName = viewModel.ProviderName, cohortRef = viewModel.CohortRef, saveStatus = SaveStatus.Save });
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/details")]
        public async Task<ActionResult> Details(string hashedAccountId, string hashedCommitmentId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await _employerCommitmentsOrchestrator.GetCommitmentDetails(hashedAccountId, hashedCommitmentId, OwinWrapper.GetClaimValue(@"sub"));

            var groupes = model.Data.ApprenticeshipGroups.Where(m => m.ShowOverlapError);
            foreach (var groupe in groupes)
            {
                var errorMessage = groupe.TrainingProgramme?.Title != null
                    ? $"{groupe.TrainingProgramme?.Title ?? ""} apprentices training dates"
                    : "Apprentices training dates";

                ModelState.AddModelError($"{groupe.GroupId}", errorMessage);
            }

            model.Data.BackLinkUrl = GetReturnToListUrl(hashedAccountId);
            SetFlashMessageOnModel(model);

            ViewBag.HashedAccountId = hashedAccountId;

            return View(model);
        }
        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/details/delete")]
        public async Task<ActionResult> DeleteCohort(string hashedAccountId, string hashedCommitmentId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await _employerCommitmentsOrchestrator.GetDeleteCommitmentModel(hashedAccountId, hashedCommitmentId, OwinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpPost]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/details/delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCohort(DeleteCommitmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var model = await _employerCommitmentsOrchestrator
                    .GetDeleteCommitmentModel(viewModel.HashedAccountId, viewModel.HashedCommitmentId, OwinWrapper.GetClaimValue(@"sub"));

                return View(model);
            }

            if (viewModel.DeleteConfirmed == null || !viewModel.DeleteConfirmed.Value)
            {
                return RedirectToAction("Details", new { viewModel.HashedAccountId, viewModel.HashedCommitmentId } );
            }

            await _employerCommitmentsOrchestrator
                .DeleteCommitment(viewModel.HashedAccountId, viewModel.HashedCommitmentId, OwinWrapper.GetClaimValue("sub"));

            var flashmessage = new FlashMessageViewModel
            {
                Message = "Cohort deleted",
                Severity = FlashMessageSeverityLevel.Okay
            };

            TempData["FlashMessage"] = JsonConvert.SerializeObject(flashmessage);

            var anyCohortWithCurrentStatus = 
                await _employerCommitmentsOrchestrator.AnyCohortsForCurrentStatus(viewModel.HashedAccountId, GetSessionRequestStatus());

            if(!anyCohortWithCurrentStatus)
                return RedirectToAction("YourCohorts", new { viewModel.HashedAccountId });

            return Redirect(GetReturnToListUrl(viewModel.HashedAccountId));

        }

        [HttpGet]
        [Route("{legalEntityCode}/AgreementNotSigned")]
        public ActionResult AgreementNotSigned(LegalEntitySignedAgreementViewModel viewModel)
        {
            return View(viewModel);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/apprenticeships/create")]
        public async Task<ActionResult> CreateApprenticeshipEntry(string hashedAccountId, string hashedCommitmentId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var response = await _employerCommitmentsOrchestrator.GetSkeletonApprenticeshipDetails(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), hashedCommitmentId);

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/apprenticeships/create")]
        public async Task<ActionResult> CreateApprenticeship(ApprenticeshipViewModel apprenticeship)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return await RedisplayCreateApprenticeshipView(apprenticeship);
                }

                await _employerCommitmentsOrchestrator.CreateApprenticeship(apprenticeship, OwinWrapper.GetClaimValue(@"sub"));
            }
            catch (InvalidRequestException ex)
            {
                AddErrorsToModelState(ex);

                return await RedisplayCreateApprenticeshipView(apprenticeship);
            }

            return RedirectToAction("Details", new { hashedAccountId = apprenticeship.HashedAccountId, hashedCommitmentId = apprenticeship.HashedCommitmentId });
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/apprenticeships/{hashedApprenticeshipId}/edit")]
        public async Task<ActionResult> EditApprenticeship(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await _employerCommitmentsOrchestrator.GetApprenticeship(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), hashedCommitmentId, hashedApprenticeshipId);
            AddErrorsToModelState(model.Data.ValidationErrors);
            return View("EditApprenticeshipEntry", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/apprenticeships/{HashedApprenticeshipId}/edit")]
        public async Task<ActionResult> EditApprenticeship(ApprenticeshipViewModel apprenticeship)
        {
            try
            {
                AddErrorsToModelState(await _employerCommitmentsOrchestrator.ValidateApprenticeship(apprenticeship));
                if (!ModelState.IsValid)
                {
                    return await RedisplayEditApprenticeshipView(apprenticeship);
                }

                await _employerCommitmentsOrchestrator.UpdateApprenticeship(apprenticeship, OwinWrapper.GetClaimValue(@"sub"));
            }
            catch (InvalidRequestException ex)
            {
                AddErrorsToModelState(ex);

                return await RedisplayEditApprenticeshipView(apprenticeship);
            }

            return RedirectToAction("Details", new { hashedAccountId = apprenticeship.HashedAccountId, hashedCommitmentId = apprenticeship.HashedCommitmentId });
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/finished")]
        public async Task<ActionResult> FinishedEditing(string hashedAccountId, string hashedCommitmentId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var response = await _employerCommitmentsOrchestrator.GetFinishEditingViewModel(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), hashedCommitmentId);

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/finished")]
        public async Task<ActionResult> FinishedEditing(FinishEditingViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var response = await _employerCommitmentsOrchestrator.GetFinishEditingViewModel(viewModel.HashedAccountId, OwinWrapper.GetClaimValue(@"sub"), viewModel.HashedCommitmentId);
                response.Data = viewModel;

                return View(response);
            }

            if(viewModel.SaveStatus.IsSend())
            {
                return RedirectToAction("SubmitExistingCommitment", 
                    new { viewModel.HashedAccountId, viewModel.HashedCommitmentId, viewModel.SaveStatus });
            }

            if (viewModel.SaveStatus.IsFinalApproval())
            {
                var userDisplayName = OwinWrapper.GetClaimValue(DasClaimTypes.DisplayName);
                var userEmail = OwinWrapper.GetClaimValue(DasClaimTypes.Email);
                var userId = OwinWrapper.GetClaimValue(@"sub");
                await _employerCommitmentsOrchestrator.ApproveCommitment(viewModel.HashedAccountId, userId, userDisplayName, userEmail, viewModel.HashedCommitmentId, viewModel.SaveStatus);

                return RedirectToAction("Approved",
                    new { viewModel.HashedAccountId, viewModel.HashedCommitmentId });
            }

            var flashmessage = new FlashMessageViewModel
            {
                Headline = "Details saved but not sent",
                Severity = FlashMessageSeverityLevel.Info
            };

            TempData["FlashMessage"] = JsonConvert.SerializeObject(flashmessage);
            return RedirectToAction("YourCohorts", new { hashedAccountId = viewModel.HashedAccountId });
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/CohortApproved")]
        public async Task<ActionResult> Approved(string hashedAccountId, string hashedCommitmentId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await _employerCommitmentsOrchestrator.GetAcknowledgementModelForExistingCommitment(
                hashedAccountId,
                hashedCommitmentId,
                OwinWrapper.GetClaimValue(@"sub"));

            var currentStatusCohortAny = await _employerCommitmentsOrchestrator
                .AnyCohortsForCurrentStatus(hashedAccountId, RequestStatus.ReadyForApproval);
            model.Data.BackLink = currentStatusCohortAny
                ? new LinkViewModel { Text = "Return to Approve cohorts", Url = Url.Action("ReadyForApproval", new { hashedAccountId }) }
                : new LinkViewModel { Text = "Return to Your cohorts", Url = Url.Action("YourCohorts", new { hashedAccountId }) };

            return View(model);
        }


        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/submit")]
        public async Task<ActionResult> SubmitExistingCommitment(string hashedAccountId, string hashedCommitmentId, SaveStatus saveStatus)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var response = await _employerCommitmentsOrchestrator.GetSubmitCommitmentModel(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), hashedCommitmentId, saveStatus);
            return View("SubmitCommitmentEntry", response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/submit")]
        public async Task<ActionResult> SubmitExistingCommitmentEntry(SubmitCommitmenViewModel model)
        {
            var userDisplayName = OwinWrapper.GetClaimValue(DasClaimTypes.DisplayName);
            var userEmail = OwinWrapper.GetClaimValue(DasClaimTypes.Email);
            var userId = OwinWrapper.GetClaimValue(@"sub");

            await _employerCommitmentsOrchestrator.SubmitCommitment(model, userId, userDisplayName, userEmail);

            return RedirectToAction("AcknowledgementExisting", new { hashedCommitmentId = model.HashedCommitmentId, model.SaveStatus });
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("Submit")]
        public async Task<ActionResult> SubmitNewCommitment(string hashedAccountId, string legalEntityCode, string legalEntityName, string legalEntityAddress, short legalEntitySource, string providerId, string providerName, string cohortRef, SaveStatus? saveStatus)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            if (string.IsNullOrWhiteSpace(legalEntityCode)
                || string.IsNullOrWhiteSpace(legalEntityName)
                || string.IsNullOrWhiteSpace(providerId)
                || string.IsNullOrWhiteSpace(providerName)
                || string.IsNullOrWhiteSpace(cohortRef)
                || string.IsNullOrWhiteSpace(legalEntityAddress)
                || !saveStatus.HasValue)
            {
                return RedirectToAction("Inform", new { hashedAccountId });
            }

            var response = await _employerCommitmentsOrchestrator.GetSubmitNewCommitmentModel
                (hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), legalEntityCode, legalEntityName, legalEntityAddress, legalEntitySource, providerId, providerName, cohortRef, saveStatus.Value);

            return View("SubmitCommitmentEntry", response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("submit")]
        public async Task<ActionResult> SubmitNewCommitmentEntry(SubmitCommitmenViewModel model)
        {
            var userDisplayName = OwinWrapper.GetClaimValue(DasClaimTypes.DisplayName);
            var userEmail = OwinWrapper.GetClaimValue(DasClaimTypes.Email);
            var userId = OwinWrapper.GetClaimValue(@"sub");

            var response = await _employerCommitmentsOrchestrator.CreateProviderAssignedCommitment(model, userId, userDisplayName, userEmail);

            return RedirectToAction("AcknowledgementNew", new { hashedAccountId = model.HashedAccountId, hashedCommitmentId = response.Data });
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/NewCohortAcknowledgement")]
        public async Task<ActionResult> AcknowledgementNew(string hashedAccountId, string hashedCommitmentId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var response = await _employerCommitmentsOrchestrator
                .GetAcknowledgementModelForExistingCommitment(hashedAccountId, hashedCommitmentId, OwinWrapper.GetClaimValue(@"sub"));

            response.Data.Content = GetAcknowledgementContent(SaveStatus.Save);

            return View("Acknowledgement", response);
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/Acknowledgement")]
        public async Task<ActionResult> AcknowledgementExisting(string hashedAccountId, string hashedCommitmentId, SaveStatus saveStatus)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var response = await _employerCommitmentsOrchestrator
                .GetAcknowledgementModelForExistingCommitment(hashedAccountId, hashedCommitmentId, OwinWrapper.GetClaimValue(@"sub"));

            var status = GetSessionRequestStatus();
            var returnToCohortsList = 
                   status != RequestStatus.None 
                && await _employerCommitmentsOrchestrator.AnyCohortsForCurrentStatus(hashedAccountId, status);

            var returnUrl = GetReturnUrl(status, hashedAccountId);
            response.Data.BackLink = string.IsNullOrEmpty(returnUrl) || !returnToCohortsList
                ? new LinkViewModel { Url = Url.Action("YourCohorts", new { hashedAccountId }), Text = "Return to Your cohorts" }
                : new LinkViewModel { Url = returnUrl, Text = "Go back to view cohorts" };

            response.Data.Content = GetAcknowledgementContent(saveStatus);
            
            

            return View("Acknowledgement", response);
        }

        private AcknowledgementContent GetAcknowledgementContent(SaveStatus saveStatus)
        {
            switch (saveStatus)
            {
                case SaveStatus.AmendAndSend:
                    return new AcknowledgementContent
                               {
                                   Title = "Cohort sent for review",
                                   Text = "Your training provider will review your cohort and contact you as soon as possible."
                               };
                case SaveStatus.ApproveAndSend:
                    return new AcknowledgementContent
                               {
                                   Title = "Cohort approved and sent to training provider",
                                   Text = "Your training provider will review your cohort and either confirm the information is correct or contact you to suggest changes."
                               };
                case SaveStatus.Save:
                    return new AcknowledgementContent
                               {
                                   Title = "Message sent",
                                   Text =
                                   "Your training provider will review your request and contact you as soon as possible - either with questions or to ask you to review the apprentice details they've added."
                               };
            }
            return new AcknowledgementContent();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/Apprenticeships/{hashedApprenticeshipId}/Delete")]
        public async Task<ActionResult> DeleteApprenticeshipConfirmation(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
            {
                return View("AccessDenied");
            }

            var response = await _employerCommitmentsOrchestrator.GetDeleteApprenticeshipViewModel(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), hashedCommitmentId, hashedApprenticeshipId);

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/Apprenticeships/{hashedApprenticeshipId}/Delete")]
        public async Task<ActionResult> DeleteApprenticeshipConfirmation(DeleteApprenticeshipConfirmationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var errorResponse = await _employerCommitmentsOrchestrator.GetDeleteApprenticeshipViewModel(viewModel.HashedAccountId, OwinWrapper.GetClaimValue(@"sub"), viewModel.HashedCommitmentId, viewModel.HashedApprenticeshipId);

                return View(errorResponse);
            }

            if (viewModel.DeleteConfirmed.HasValue && viewModel.DeleteConfirmed.Value)
            {
                await _employerCommitmentsOrchestrator.DeleteApprenticeship(viewModel, OwinWrapper.GetClaimValue(@"sub"));

                var flashMessage = new FlashMessageViewModel { Severity = FlashMessageSeverityLevel.Okay, Message = string.Format($"Apprentice record for {viewModel.ApprenticeshipName} deleted") };
                TempData["FlashMessage"] = JsonConvert.SerializeObject(flashMessage);

                return RedirectToAction("Details", new { viewModel.HashedAccountId, viewModel.HashedCommitmentId });
            }

            return RedirectToAction("EditApprenticeship", new { viewModel.HashedAccountId, viewModel.HashedCommitmentId, viewModel.HashedApprenticeshipId });
        }

        private RequestStatus GetSessionRequestStatus()
        {
            var status = (RequestStatus?)Session[LastCohortPageSessionKey] ?? RequestStatus.None;
            return status;
        }

        private string GetReturnUrl(RequestStatus status, string hashedAccountId)
        {
            switch (status)
            {
                case RequestStatus.NewRequest:
                    return Url.Action("WaitingToBeSent", new { hashedAccountId });
                case RequestStatus.ReadyForReview:
                    return Url.Action("ReadyForReview", new { hashedAccountId });
                case RequestStatus.ReadyForApproval:
                    return Url.Action("ReadyForApproval", new { hashedAccountId });
                default:
                    return string.Empty;
            }
        }

        private void AddErrorsToModelState(InvalidRequestException ex)
        {
            foreach (var error in ex.ErrorMessages)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
        }

        private void AddErrorsToModelState(Dictionary<string, string> dict)
        {
            foreach (var error in dict)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
        }

        private async Task<ActionResult> RedisplayCreateApprenticeshipView(ApprenticeshipViewModel apprenticeship)
        {
            var response = await _employerCommitmentsOrchestrator.GetSkeletonApprenticeshipDetails(apprenticeship.HashedAccountId, OwinWrapper.GetClaimValue(@"sub"), apprenticeship.HashedCommitmentId);
            response.Data.Apprenticeship = apprenticeship;

            return View("CreateApprenticeshipEntry", response);
        }

        private async Task<ActionResult> RedisplayEditApprenticeshipView(ApprenticeshipViewModel apprenticeship)
        {
            var response = await _employerCommitmentsOrchestrator.GetSkeletonApprenticeshipDetails(apprenticeship.HashedAccountId, OwinWrapper.GetClaimValue(@"sub"), apprenticeship.HashedCommitmentId);
            response.Data.Apprenticeship = apprenticeship;

            return View("EditApprenticeshipEntry", response);
        }

        private string GetReturnToListUrl(string hashedAccountId)
        {
            switch (GetSessionRequestStatus())
            {
                case RequestStatus.WithProviderForApproval:
                case RequestStatus.SentForReview:
                    return Url.Action("WithProvider", new { hashedAccountId });
                case RequestStatus.NewRequest:
                    return Url.Action("WaitingToBeSent", new { hashedAccountId });
                case RequestStatus.ReadyForReview:
                    return Url.Action("ReadyForReview", new { hashedAccountId });
                case RequestStatus.ReadyForApproval:
                    return Url.Action("ReadyForApproval", new { hashedAccountId });
                default:
                    return Url.Action("YourCohorts", new { hashedAccountId });
            }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is InvalidStateException)
            {
                var hashedAccountId = filterContext.RouteData.Values["hashedAccountId"];

                filterContext.ExceptionHandled = true;
                filterContext.Result = RedirectToAction("InvalidState", "Error", new { hashedAccountId });
            }
        }

        private void SetFlashMessageOnModel<T>(OrchestratorResponse<T> model)
        {
            if (!string.IsNullOrEmpty(TempData["FlashMessage"]?.ToString()))
            {
                var flashMessage = JsonConvert.DeserializeObject<FlashMessageViewModel>(TempData["FlashMessage"].ToString());
                model.FlashMessage = flashMessage;
            }
        }

        private async Task<bool> IsUserRoleAuthorized(string hashedAccountId, params Role[] roles)
        {
            return await _employerCommitmentsOrchestrator.AuthorizeRole(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), roles);
        }
    }
}