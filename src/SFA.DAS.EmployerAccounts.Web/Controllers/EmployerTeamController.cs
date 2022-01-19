﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerAccounts.Helpers;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [DasAuthorize]
    [RoutePrefix("accounts/{HashedAccountId}/teams")]
    public class EmployerTeamController : BaseController
    {
        private readonly EmployerTeamOrchestrator _employerTeamOrchestrator;
   
        public EmployerTeamController(
            IAuthenticationService owinWrapper)
            : base(owinWrapper)
        {
            _employerTeamOrchestrator = null;            
        }

        public EmployerTeamController(
            IAuthenticationService owinWrapper,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage,
            EmployerTeamOrchestrator employerTeamOrchestrator)
            : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            _employerTeamOrchestrator = employerTeamOrchestrator;
        }

        [HttpGet]
        [Route]
        public async Task<ActionResult> Index(string hashedAccountId, string reservationId)
        {
            PopulateViewBagWithExternalUserId();            
            var response = await GetAccountInformation(hashedAccountId);
            
            if (response.Status != HttpStatusCode.OK)
            {
                return View(response);
            }

            if (!response.Data.HasPayeScheme)
            {
                ViewBag.HideNav = true;
            }

            return View(response);
        }

        [HttpGet]
        [Route("view")]
        public async Task<ActionResult> ViewTeam(string hashedAccountId)
        {
            var response = await _employerTeamOrchestrator.GetTeamMembers(hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            var flashMessage = GetFlashMessageViewModelFromCookie();
            if (flashMessage != null)
            {
                response.FlashMessage = flashMessage;
            }

            return View(response);
        }

        [HttpGet]
        [Route("AddedProvider/{providerName}")]
        public async Task<ActionResult> AddedProvider(string providerName)
        {
            AddFlashMessageToCookie(new FlashMessageViewModel
            {
                Headline = "Your account has been created",
                Message = $"You account has been created and you've successfully updated permissions for {HttpUtility.UrlDecode(providerName.ToUpper())}",
                Severity = FlashMessageSeverityLevel.Success
            });

            return RedirectToAction(ControllerConstants.IndexActionName);
        }

        [HttpGet]
        [Route("invite")]
        public async Task<ActionResult> Invite(string hashedAccountId)
        {
            var response = await _employerTeamOrchestrator.GetNewInvitation(hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("invite")]
        public async Task<ActionResult> Invite(InviteTeamMemberViewModel model)
        {
            var response = await _employerTeamOrchestrator.InviteTeamMember(model, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            if (response.Status == HttpStatusCode.OK)
            {
                var flashMessage = new FlashMessageViewModel
                {
                    HiddenFlashMessageInformation = "page-invite-team-member-sent",
                    Severity = FlashMessageSeverityLevel.Success,
                    Headline = "Invitation sent",
                    Message = $"You've sent an invitation to <strong>{model.Email}</strong>"
                };
                AddFlashMessageToCookie(flashMessage);

                return RedirectToAction(ControllerConstants.NextStepsActionName);
            }


            model.ErrorDictionary = response.FlashMessage.ErrorMessages;
            var errorResponse = new OrchestratorResponse<InviteTeamMemberViewModel>
            {
                Data = model,
                FlashMessage = response.FlashMessage,
            };

            return View(errorResponse);
        }

        [HttpGet]
        [Route("invite/next")]
        public async Task<ActionResult> NextSteps(string hashedAccountId)
        {
            var userId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);

            var userShownWizard = await _employerTeamOrchestrator.UserShownWizard(userId, hashedAccountId);

            var model = new OrchestratorResponse<InviteTeamMemberNextStepsViewModel>
            {
                FlashMessage = GetFlashMessageViewModelFromCookie(),
                Data = new InviteTeamMemberNextStepsViewModel
                {
                    UserShownWizard = userShownWizard,
                    HashedAccountId = hashedAccountId
                }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("invite/next")]
        public async Task<ActionResult> NextSteps(int? choice, string hashedAccountId)
        {
            var userId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);

            var userShownWizard = await _employerTeamOrchestrator.UserShownWizard(userId, hashedAccountId);

            switch (choice ?? 0)
            {
                case 1: return RedirectToAction(ControllerConstants.InviteActionName);
                case 2: return RedirectToAction(ControllerConstants.ViewTeamActionName);
                case 3: return RedirectToAction(ControllerConstants.IndexActionName);
                default:
                    var model = new OrchestratorResponse<InviteTeamMemberNextStepsViewModel>
                    {
                        FlashMessage = GetFlashMessageViewModelFromCookie(),
                        Data = new InviteTeamMemberNextStepsViewModel
                        {
                            ErrorMessage = "You must select an option to continue.",
                            UserShownWizard = userShownWizard
                        }
                    };
                    return View(model); //No option entered
            }
        }

        [HttpGet]
        [Route("{invitationId}/cancel")]
        public async Task<ActionResult> Cancel(string email, string invitationId, string hashedAccountId)
        {
            var invitation = await _employerTeamOrchestrator.GetInvitation(invitationId);
            invitation.Data.HashedAccountId = hashedAccountId;

            return View(invitation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{invitationId}/cancel")]
        public async Task<ActionResult> Cancel(string invitationId, string email, string hashedAccountId, int cancel)
        {
            if (cancel != 1)
                return RedirectToAction(ControllerConstants.ViewTeamViewName, new { HashedAccountId = hashedAccountId });

            var response = await _employerTeamOrchestrator.Cancel(email, hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            return View(ControllerConstants.ViewTeamViewName, response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("resend")]
        public async Task<ActionResult> Resend(string hashedAccountId, string email, string name)
        {
            var response = await _employerTeamOrchestrator.Resend(email, hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName), name);

            return View(ControllerConstants.ViewTeamViewName, response);
        }

        [HttpGet]
        [Route("{email}/remove")]
        public async Task<ActionResult> Remove(string hashedAccountId, string email)
        {
            var response = await _employerTeamOrchestrator.Review(hashedAccountId, email);

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{email}/remove")]
        public async Task<ActionResult> Remove(long userId, string hashedAccountId, string email, int remove)
        {
            Exception exception;
            HttpStatusCode httpStatusCode;

            try
            {
                if (remove != 1)
                    return RedirectToAction(ControllerConstants.ViewTeamViewName, new { HashedAccountId = hashedAccountId });

                var response = await _employerTeamOrchestrator.Remove(userId, hashedAccountId, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

                return View(ControllerConstants.ViewTeamViewName, response);
            }
            catch (InvalidRequestException e)
            {
                httpStatusCode = HttpStatusCode.BadRequest;
                exception = e;
            }
            catch (UnauthorizedAccessException e)
            {
                httpStatusCode = HttpStatusCode.Unauthorized;
                exception = e;
            }

            var errorResponse = await _employerTeamOrchestrator.Review(hashedAccountId, email);
            errorResponse.Status = httpStatusCode;
            errorResponse.Exception = exception;

            return View(errorResponse);
        }

        [HttpGet]
        [Route("{email}/role/change")]
        public async Task<ActionResult> ChangeRole(string hashedAccountId, string email)
        {
            var teamMember = await _employerTeamOrchestrator.GetTeamMemberWhetherActiveOrNot(hashedAccountId, email, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            return View(teamMember);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{email}/role/change")]
        public async Task<ActionResult> ChangeRole(string hashedAccountId, string email, Role role)
        {
            var response = await _employerTeamOrchestrator.ChangeRole(hashedAccountId, email, role, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            if (response.Status == HttpStatusCode.OK)
            {
                return View(ControllerConstants.ViewTeamViewName, response);
            }

            var teamMemberResponse = await _employerTeamOrchestrator.GetTeamMemberWhetherActiveOrNot(hashedAccountId, email, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            //We have to override flash message as the change role view has different model to view team view
            teamMemberResponse.FlashMessage = response.FlashMessage;
            teamMemberResponse.Exception = response.Exception;

            return View(teamMemberResponse);
        }

        [HttpGet]
        [Route("{email}/review")]
        public async Task<ActionResult> Review(string hashedAccountId, string email)
        {
            var invitation = await _employerTeamOrchestrator.GetTeamMemberWhetherActiveOrNot(hashedAccountId, email, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            return View(invitation);
        }

        [HttpGet]
        [Route("hideWizard")]
        public async Task<ActionResult> HideWizard(string hashedAccountId)
        {
            var externalUserId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);

            await _employerTeamOrchestrator.HideWizard(hashedAccountId, externalUserId);

            return RedirectToAction(ControllerConstants.IndexActionName);
        }

        [HttpGet]
        [Route("continuesetupcreateadvert")]
        public ActionResult ContinueSetupCreateAdvert(string hashedAccountId)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("continuesetupcreateadvert")]
        public ActionResult ContinueSetupCreateAdvert(string hashedAccountId, bool? requiresAdvert)
        {
            if (!requiresAdvert.HasValue)
            {
                ViewData.ModelState.AddModelError("Choice", "You must select an option to continue.");
                return View();
            }
            else if(requiresAdvert.Value == true)
            {
                return Redirect(Url.EmployerRecruitAction());
            }

            return Redirect(Url.EmployerCommitmentsV2Action("unapproved/Inform"));
        }

        [ChildActionOnly]
        public ActionResult SingleApprenticeshipContinueSetup(AccountDashboardViewModel model)
        {
            return PartialView(model.CallToActionViewModel.Cohorts.Single().Apprenticeships.Single());
        }

        [ChildActionOnly]
        public ActionResult SingleApprenticeshipWithTrainingProvider(AccountDashboardViewModel model)
        {
            return PartialView(model.CallToActionViewModel.Cohorts.Single().Apprenticeships.Single());
        }

        [ChildActionOnly]
        public ActionResult SingleApprenticeshipReadyForReview(AccountDashboardViewModel model)
        {
            return PartialView(model.CallToActionViewModel.Cohorts.Single().Apprenticeships.Single());
        }

        [ChildActionOnly]
        public ActionResult SingleApprenticeshipApproved(AccountDashboardViewModel model)
        {
            return PartialView(model.CallToActionViewModel.Apprenticeships.First());
        }

        [ChildActionOnly]
        public ActionResult SingleApprenticeshipContinueWithProvider(AccountDashboardViewModel model)
        {
            model.CallToActionViewModel.Cohorts.Single().Apprenticeships = new List<ApprenticeshipViewModel>()
            {
                new ApprenticeshipViewModel()
                {
                    CourseName = model.CallToActionViewModel.Cohorts?.Single()?.CohortApprenticeshipsCount > 0 ? model.CallToActionViewModel.Cohorts?.Single()?.Apprenticeships?.Single()?.CourseName : string.Empty,
                    HashedCohortId = model.CallToActionViewModel.Cohorts?.Single().HashedCohortId,
                    TrainingProvider = model.CallToActionViewModel.Cohorts?.Single().TrainingProvider.First()
                }
            };          
            return PartialView(model.CallToActionViewModel.Cohorts.Single().Apprenticeships.Single());
        }

        [ChildActionOnly]
        public override ActionResult SupportUserBanner(IAccountIdentifier model = null)
        {
            EmployerAccounts.Models.Account.Account account = null;

            if (model != null && model.HashedAccountId != null)
            {
                var externalUserId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
                var response = AsyncHelper.RunSync(() => _employerTeamOrchestrator.GetAccountSummary(model.HashedAccountId, externalUserId));
                account = response.Status != HttpStatusCode.OK ? null : response.Data.Account;
            }

            var consoleUserType = OwinWrapper.GetClaimValue(ClaimTypes.Role) == "Tier2User" ? "Service user (T2 Support)" : "Standard user";
            
            return PartialView("_SupportUserBanner", new SupportUserBannerViewModel
            {
                Account = account,
                ConsoleUserType = consoleUserType
            });
        }

        [ChildActionOnly]
        public ActionResult Row1Panel1(AccountDashboardViewModel model)
        {
            var viewModel = new PanelViewModel<AccountDashboardViewModel> { ViewName = "Empty", Data = model };

            if (model.PayeSchemeCount == 0)
            {
                viewModel.ViewName = "AddPAYE";                
            }
            else
            {
                _employerTeamOrchestrator.GetCallToActionViewName(viewModel);
            }

            return PartialView(viewModel);
        }

        [ChildActionOnly]
        public ActionResult AddPAYE(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult SignAgreement(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }        

        [ChildActionOnly]
        public ActionResult Empty(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult CheckFunding(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult ContinueSetupForSingleReservation(AccountDashboardViewModel model)
        {
            var reservation = model.CallToActionViewModel.Reservations?.FirstOrDefault();
            var viewModel = new ReservationViewModel(reservation);
            return PartialView(viewModel);
        }

        [ChildActionOnly]
        public ActionResult VacancyDraft(AccountDashboardViewModel model)
        {   
            return PartialView(model.CallToActionViewModel.VacanciesViewModel.Vacancies.First(m => m.Status == EmployerAccounts.Models.Recruit.VacancyStatus.Draft));
        }

        [ChildActionOnly]
        public ActionResult VacancyPendingReview(AccountDashboardViewModel model)
        {
            return PartialView(model.CallToActionViewModel.VacanciesViewModel.Vacancies.First(m => m.Status == EmployerAccounts.Models.Recruit.VacancyStatus.Submitted));
        }

        [ChildActionOnly]
        public ActionResult VacancyRejected(AccountDashboardViewModel model)
        {
            return PartialView(model.CallToActionViewModel.VacanciesViewModel.Vacancies.First(m => m.Status == EmployerAccounts.Models.Recruit.VacancyStatus.Referred));
        }

        [ChildActionOnly]
        public ActionResult VacancyLive(AccountDashboardViewModel model)
        {
            return PartialView(model.CallToActionViewModel.VacanciesViewModel.Vacancies.First(m => m.Status == EmployerAccounts.Models.Recruit.VacancyStatus.Live));
        }

        [ChildActionOnly]
        public ActionResult VacancyClosed(AccountDashboardViewModel model)
        {
            return PartialView(model.CallToActionViewModel.VacanciesViewModel.Vacancies.First(m => m.Status == EmployerAccounts.Models.Recruit.VacancyStatus.Closed));
        }

        [HttpGet]
        [Route("triagewhichcourseyourapprenticewilltake")]
        public ActionResult TriageWhichCourseYourApprenticeWillTake()
        {
            return View();
        }

        [HttpPost]
        [Route("triagewhichcourseyourapprenticewilltake")]
        [ValidateAntiForgeryToken]
        public ActionResult TriageWhichCourseYourApprenticeWillTake(TriageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            switch (model.TriageOption)
            {
                case TriageOptions.Yes:
                {
                    return RedirectToAction(ControllerConstants.TriageHaveYouChosenATrainingProviderActionName);
                }

                case TriageOptions.No:
                {
                    return RedirectToAction(ControllerConstants.TriageYouCannotSetupAnApprenticeshipYetCourseProviderActionName);
                }

                default:
                {
                    return View(model);
                }
            }
        }

        [HttpGet]
        [Route("triageyoucannotsetupanapprenticeshipyetcourseprovider")]
        public ActionResult TriageYouCannotSetupAnApprenticeshipYetCourseProvider()
        {
            return View();
        }

        [HttpGet]
        [Route("triagehaveyouchosenatrainingprovider")]
        public ActionResult TriageHaveYouChosenATrainingProvider()
        {
            return View();
        }

        [HttpPost]
        [Route("triagehaveyouchosenatrainingprovider")]
        [ValidateAntiForgeryToken]
        public ActionResult TriageHaveYouChosenATrainingProvider(TriageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            switch (model.TriageOption)
            {
                case TriageOptions.Yes:
                {
                    return RedirectToAction(ControllerConstants.TriageWillApprenticeshipTrainingStartActionName);
                }

                case TriageOptions.No:
                {
                    return RedirectToAction(ControllerConstants.TriageYouCannotSetupAnApprenticeshipYetProviderActionName);
                }

                default:
                {
                    return View(model);
                }
            }
        }

        [HttpGet]
        [Route("triageyoucannotsetupanapprenticeshipyetprovider")]
        public ActionResult TriageYouCannotSetupAnApprenticeshipYetProvider()
        {
            return View();
        }

        [HttpGet]
        [Route("triagewillapprenticeshiptrainingstart")]
        public ActionResult TriageWillApprenticeshipTrainingStart()
        {
            return View();
        }

        [HttpPost]
        [Route("triagewillapprenticeshiptrainingstart")]
        [ValidateAntiForgeryToken]
        public ActionResult TriageWillApprenticeshipTrainingStart(TriageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            switch (model.TriageOption)
            {
                case TriageOptions.Yes:
                {
                    return RedirectToAction(ControllerConstants.TriageApprenticeForExistingEmployeeActionName);
                }

                case TriageOptions.No:
                {
                    return RedirectToAction(ControllerConstants.TriageYouCannotSetupAnApprenticeshipYetStartDateActionName);
                }

                case TriageOptions.Unknown:
                {
                    return RedirectToAction(ControllerConstants.TriageYouCannotSetupAnApprenticeshipYetApproximateStartDateActionName);
                }

                default:
                {
                    return View(model);
                }
            }
        }

        [HttpGet]
        [Route("triageyoucannotsetupanapprenticeshipyetstartdate")]
        public ActionResult TriageYouCannotSetupAnApprenticeshipYetStartDate()
        {
            return View();
        }

        [HttpGet]
        [Route("triageyoucannotsetupanapprenticeshipyetapproximatestartdate")]
        public ActionResult TriageYouCannotSetupAnApprenticeshipYetApproximateStartDate()
        {
            return View();
        }

        [HttpGet]
        [Route("triageapprenticeforexistingemployee")]
        public ActionResult TriageApprenticeForExistingEmployee()
        {
            return View();
        }

        [HttpPost]
        [Route("triageapprenticeforexistingemployee")]
        [ValidateAntiForgeryToken]
        public ActionResult TriageApprenticeForExistingEmployee(TriageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            switch (model.TriageOption)
            {
                case TriageOptions.Yes:
                {
                    return View(ControllerConstants.TriageSetupApprenticeshipExistingEmployeeViewName);
                }

                case TriageOptions.No:
                {
                    return View(ControllerConstants.TriageSetupApprenticeshipNewEmployeeViewName);
                }

                default:
                {
                    return View(model);
                }
            }
        }

        private async Task<OrchestratorResponse<AccountDashboardViewModel>> GetAccountInformation(string hashedAccountId)
        {
            var externalUserId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
            var response = await _employerTeamOrchestrator.GetAccount(hashedAccountId, externalUserId);

            var flashMessage = GetFlashMessageViewModelFromCookie();

            if (flashMessage != null)
            {
                response.FlashMessage = flashMessage;
                response.Data.EmployerAccountType = flashMessage.HiddenFlashMessageInformation;
            }

            return response;
        }

        private void PopulateViewBagWithExternalUserId()
        {
            var externalUserId = OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName);
            if (externalUserId != null)
                ViewBag.UserId = externalUserId;
        }
    }
}
