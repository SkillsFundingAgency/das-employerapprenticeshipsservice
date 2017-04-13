using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;
using WebGrease.Css.Extensions;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;
using FluentValidation.Mvc;
using SFA.DAS.EAS.Web.ViewModels;
using Newtonsoft.Json;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{hashedAccountId}/apprentices/manage")]
    public class EmployerManageApprenticesController : BaseController
    {
        private readonly EmployerManageApprenticeshipsOrchestrator _orchestrator;

        public EmployerManageApprenticesController(
            EmployerManageApprenticeshipsOrchestrator orchestrator, 
            IOwinWrapper owinWrapper,
            IFeatureToggle featureToggle,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage)
                : base(owinWrapper, featureToggle, multiVariantTestingService, flashMessage)
        {
            if (orchestrator == null)
                throw new ArgumentNullException(nameof(orchestrator));

            _orchestrator = orchestrator;
        }

        [HttpGet]
        [Route("all")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> ListAll(string hashedAccountId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await _orchestrator
                .GetApprenticeships(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedApprenticeshipId}/details", Name = "OnProgrammeApprenticeshipDetails")]
        public async Task<ActionResult> Details(string hashedAccountId, string hashedApprenticeshipId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await _orchestrator
                .GetApprenticeship(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));
            var flashMessage = GetFlashMessageViewModelFromCookie();

            if (flashMessage != null )
            {
                model.FlashMessage = flashMessage;
            }

            return View(model);
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/details/statuschange", Name = "ChangeStatusSelectOption")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> ChangeStatus(string hashedAccountId, string hashedApprenticeshipId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            // Only used for authorization and checking status of apprenticeship
            var response = await _orchestrator.GetChangeStatusChoiceNavigation(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));

            return View(response);
        }

        [HttpPost]
        [Route("{hashedApprenticeshipId}/details/statuschange", Name = "PostChangeStatusSelectOption")]
        public async Task<ActionResult> ChangeStatus(string hashedAccountId, string hashedApprenticeshipId, ChangeStatusViewModel model)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            if (!ModelState.IsValid)
            {
                return View(new OrchestratorResponse<ChangeStatusViewModel>());
            }

            if (model.ChangeType == ChangeStatusType.None)
                return RedirectToRoute("OnProgrammeApprenticeshipDetails");

            return RedirectToRoute("WhenToApplyChange", new { changeType = model.ChangeType.ToString().ToLower() });
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/details/statuschange/{changeType}/whentoapply", Name = "WhenToApplyChange")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> WhenToApplyChange(string hashedAccountId, string hashedApprenticeshipId, ChangeStatusType changeType)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var response = await _orchestrator.GetChangeStatusDateOfChangeViewModel(hashedAccountId, hashedApprenticeshipId, changeType, OwinWrapper.GetClaimValue(@"sub"));

            if (response.Data.SkipStep)
                return RedirectToRoute("StatusChangeConfirmation", new
                {
                    changeType = response.Data.ChangeStatusViewModel.ChangeType.ToString().ToLower(),
                    whenToMakeChange = WhenToMakeChangeOptions.Immediately,
                    dateOfChange = default(DateTime?)
                });

            return View(new OrchestratorResponse<WhenToMakeChangeViewModel>
            {
                Data = response.Data
            });
        }

        [HttpPost]
        [Route("{hashedApprenticeshipId}/details/statuschange/{changeType}/whentoapply", Name = "PostWhenToApplyChange")]
        public async Task<ActionResult> WhenToApplyChange(string hashedAccountId, string hashedApprenticeshipId, [CustomizeValidator(RuleSet = "default,Date")] ChangeStatusViewModel model)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            if (!ModelState.IsValid)
            {
                var viewResponse = await _orchestrator.GetChangeStatusDateOfChangeViewModel(hashedAccountId, hashedApprenticeshipId, model.ChangeType.Value, OwinWrapper.GetClaimValue(@"sub"));

                return View(new OrchestratorResponse<WhenToMakeChangeViewModel>() { Data = viewResponse.Data });
            }

            var response = await _orchestrator.ValidateWhenToApplyChange(hashedAccountId, hashedApprenticeshipId, model);

            if (!response.ValidationResult.IsValid())
            {
                response.ValidationResult.AddToModelState(ModelState);

                var viewResponse = await _orchestrator.GetChangeStatusDateOfChangeViewModel(hashedAccountId, hashedApprenticeshipId, model.ChangeType.Value, OwinWrapper.GetClaimValue(@"sub"));

                return View(new OrchestratorResponse<WhenToMakeChangeViewModel>() { Data = viewResponse.Data });
            }

            return RedirectToRoute("StatusChangeConfirmation",  new { whenToMakeChange = model.WhenToMakeChange, dateOfChange = response.DateOfChange });
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/details/statuschange/{changeType}/confirm", Name = "StatusChangeConfirmation")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> StatusChangeConfirmation(string hashedAccountId, string hashedApprenticeshipId, ChangeStatusType changeType, WhenToMakeChangeOptions whenToMakeChange, DateTime? dateOfChange)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var response = await _orchestrator.GetChangeStatusConfirmationViewModel(hashedAccountId, hashedApprenticeshipId, changeType, whenToMakeChange, dateOfChange, OwinWrapper.GetClaimValue(@"sub"));

            return View(response);
        }

        [HttpPost]
        [Route("{hashedApprenticeshipId}/details/statuschange/{changeType}/confirm", Name = "PostStatusChangeConfirmation")]
        public async Task<ActionResult> StatusChangeConfirmation(string hashedAccountId, string hashedApprenticeshipId, [CustomizeValidator(RuleSet = "default,Date,Confirm")] ChangeStatusViewModel model)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            if (!ModelState.IsValid)
            {
                return View(new OrchestratorResponse<ConfirmationStateChangeViewModel> { Data = new ConfirmationStateChangeViewModel { ApprenticeName = "Fred", DateOfBirth = new DateTime(1977, 3, 4), ChangeStatusViewModel = model } });
            }

            if (model.ChangeConfirmed.HasValue && !model.ChangeConfirmed.Value)
                return RedirectToRoute("OnProgrammeApprenticeshipDetails");

            await _orchestrator.UpdateStatus(hashedAccountId, hashedApprenticeshipId, model, OwinWrapper.GetClaimValue(@"sub"));

            var flashmessage = new FlashMessageViewModel
            {
                Message = "Apprentice stopped.",
                Severity = FlashMessageSeverityLevel.Okay
            };

            TempData["FlashMessage"] = JsonConvert.SerializeObject(flashmessage);

            return RedirectToRoute("OnProgrammeApprenticeshipDetails");
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedApprenticeshipId}/edit", Name = "EditApprovedApprentice")]
        public async Task<ActionResult> Edit(string hashedAccountId, string hashedApprenticeshipId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await _orchestrator.GetApprenticeshipForEdit(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));

            var flashMessage = GetFlashMessageViewModelFromCookie();

            if (flashMessage?.ErrorMessages != null && flashMessage.ErrorMessages.Any())
            {
                model.FlashMessage = flashMessage;
                model.Data.Apprenticeship.ErrorDictionary = flashMessage.ErrorMessages;
            }
            
            return View(model);
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/changes/confirm")]
        public async Task<ActionResult> ConfirmChanges(string hashedAccountId, string hashedApprenticeshipId)
        {
            var model = await _orchestrator.GetOrchestratorResponseUpdateApprenticeshipViewModelFromCookie(hashedAccountId, hashedApprenticeshipId);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedApprenticeshipId}/changes/confirm")]
        public async Task<ActionResult> ConfirmChanges(ApprenticeshipViewModel apprenticeship)
        {
            if (!await IsUserRoleAuthorized(apprenticeship.HashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            if (!ModelState.IsValid)
            {
                AddErrorsToFlashDictionaryCookie();
                return RedirectToAction("Edit");
            }

            var dictionary = await _orchestrator.ValidateApprenticeship(apprenticeship);
            
            if (dictionary.Any())
            {
                AddErrorsToFlashDictionaryCookie(dictionary);
                return RedirectToAction("Edit");
            }
            
            var model = await _orchestrator.GetConfirmChangesModel(apprenticeship.HashedAccountId, apprenticeship.HashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"), apprenticeship);

            if (!AnyChanges(model.Data))
            {
                AddErrorsToFlashDictionaryCookie(new Dictionary<string, string> { { "NoChangesRequested", "No changes made" } });
                return RedirectToAction("Edit");
            }

            _orchestrator.CreateApprenticeshipViewModelCookie(model.Data);

            return RedirectToAction("ConfirmChanges");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedApprenticeshipId}/changes/SubmitChanges")]
        public async Task<ActionResult> SubmitChanges(string hashedAccountId, string hashedApprenticeshipId, UpdateApprenticeshipViewModel apprenticeship, string originalApprenticeshipDecoded)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var originalApprenticeship = System.Web.Helpers.Json.Decode<Apprenticeship>(originalApprenticeshipDecoded);
            apprenticeship.OriginalApprenticeship = originalApprenticeship;

            if (!ModelState.IsValid)
            {
                AddErrorsToFlashDictionaryCookie();
                return RedirectToAction("ConfirmChanges");
            }

            if (apprenticeship.ChangesConfirmed != null && !apprenticeship.ChangesConfirmed.Value)
            {
                return RedirectToAction("Details", new { hashedAccountId, hashedApprenticeshipId });
            }
            
            await _orchestrator.CreateApprenticeshipUpdate(apprenticeship, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            var flashmessage = new FlashMessageViewModel
            {
                Message = $"You suggested changes to the record for {originalApprenticeship.FirstName} {originalApprenticeship.LastName}. Your training provider needs to approve these changes.",
                Severity = FlashMessageSeverityLevel.Okay
            };

            AddFlashMessageToCookie(flashmessage);


            return RedirectToAction("Details", new { hashedAccountId, hashedApprenticeshipId });
        }


        [HttpGet]
        [Route("{hashedApprenticeshipId}/changes/view", Name = "ViewPendingChanges")]
        public async Task<ActionResult> ViewChanges(string hashedAccountId, string hashedApprenticeshipId)
        {
            var viewModel = await _orchestrator
                .GetViewChangesViewModel(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));
            return View(viewModel);
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedApprenticeshipId}/changes/view")]
        public async Task<ActionResult> ViewChanges(string hashedAccountId, string hashedApprenticeshipId, UpdateApprenticeshipViewModel apprenticeship, string originalApprenticeshipDecoded, bool? undoChanges)
        {
            if (undoChanges == null)
            {
                var originalApprenticeship = System.Web.Helpers.Json.Decode<Apprenticeship>(originalApprenticeshipDecoded);
                apprenticeship.OriginalApprenticeship = originalApprenticeship;
                return View(new OrchestratorResponse<UpdateApprenticeshipViewModel> { Data = apprenticeship });
            }

            if (undoChanges.Value)
            {
                await _orchestrator.SubmitUndoApprenticeshipUpdate(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));
                SetOkayMessage("Changes undone");
            }
            
            return RedirectToAction("Details");
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/changes/review", Name = "ReviewChanges")]
        public async Task<ActionResult> ReviewChanges(string hashedAccountId, string hashedApprenticeshipId)
        {
            var viewModel = await _orchestrator
                .GetViewChangesViewModel(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedApprenticeshipId}/changes/review")]
        public async Task<ActionResult> ReviewChanges(string hashedAccountId, string hashedApprenticeshipId, bool? approveChanges)
        {
            var viewModel = await _orchestrator
                .GetViewChangesViewModel(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));

            if (approveChanges == null)
                return View(viewModel);

            await _orchestrator.SubmitReviewApprenticeshipUpdate(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"), approveChanges.Value);

            var message = approveChanges.Value ? "Changes approved" : "Changes rejected";
            SetOkayMessage(message);
            return RedirectToAction("Details", new { hashedAccountId, hashedApprenticeshipId });
        }

        
        private bool AnyChanges(UpdateApprenticeshipViewModel data)
        {
            return
                   !string.IsNullOrEmpty(data.FirstName)
                || !string.IsNullOrEmpty(data.LastName)
                || data.DateOfBirth != null
                || !string.IsNullOrEmpty(data.TrainingName)
                || data.StartDate != null
                || data.EndDate != null
                || data.Cost != null
                || !string.IsNullOrEmpty(data.EmployerRef);
        }

        private async Task<bool> IsUserRoleAuthorized(string hashedAccountId, params Role[] roles)
        {
            return await _orchestrator
                .AuthorizeRole(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), roles);
        }

        private void AddErrorsToFlashDictionaryCookie()
        {
            var errorDictionary = ModelState
                                    .Where(c=>c.Value.Errors.Any())
                                    .ToDictionary(errorKeyValuePair => 
                                                        errorKeyValuePair.Key, 
                                                        errorMessage => errorMessage.Value.Errors.FirstOrDefault()?.ErrorMessage ?? "Error");

            AddErrorsToFlashDictionaryCookie(errorDictionary);
        }

        private void AddErrorsToFlashDictionaryCookie(Dictionary<string, string> dict)
        {
            var flashMessage = new FlashMessageViewModel
            {
                ErrorMessages = dict,
                Headline = "Errors to fix",
                Message = "Check the following details:",
                Severity = FlashMessageSeverityLevel.Error
            };

            AddFlashMessageToCookie(flashMessage);
        }

        private void SetOkayMessage(string message)
        {
            var flashmessage = new FlashMessageViewModel
            {
                Message = message,
                Severity = FlashMessageSeverityLevel.Okay
            };
            TempData["FlashMessage"] = JsonConvert.SerializeObject(flashmessage);

            AddFlashMessageToCookie(flashmessage);
        }
    }
}