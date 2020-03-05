using SFA.DAS.Authentication;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EAS.Portal.Client;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Validation;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [DasAuthorize]
    [RoutePrefix("accounts/{HashedAccountId}/teams")]
    public class EmployerTeamController : BaseController
    {
        private readonly EmployerTeamOrchestrator _employerTeamOrchestrator;        
        private readonly IPortalClient _portalClient;
        private readonly IAuthorizationService _authorizationService;

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
            EmployerTeamOrchestrator employerTeamOrchestrator,            
            IPortalClient portalClient,
            IAuthorizationService authorizationService)
            : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            _employerTeamOrchestrator = employerTeamOrchestrator;            
            _portalClient = portalClient;
            _authorizationService = authorizationService;
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

            return Redirect(Url.EmployerCommitmentsAction("apprentices/inform"));
        }

        [ChildActionOnly]
        public ActionResult SingleApprenticeshipContinueSetup(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult SingleApprenticeshipWithTrainingProvider(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult SingleApprenticeshipReadyForReview(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult SingleApprenticeshipApproved(AccountDashboardViewModel model)
        {
            return PartialView(model.CallToActionViewModel.Apprenticeships.First());
        }

        [ChildActionOnly]
        public ActionResult SingleApprenticeshipContinueWithProvider(AccountDashboardViewModel model)
        {          
            model.CallToActionViewModel.Cohorts.Single().Apprenticeships.Single().CourseName =  model.CallToActionViewModel.Reservations?.Single().Course?.CourseDescription;
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

            return PartialView("_SupportUserBanner", new SupportUserBannerViewModel() { Account = account });
        }

        [ChildActionOnly]
        public ActionResult Row1Panel1(AccountDashboardViewModel model)
        {
            var viewModel = new PanelViewModel<AccountDashboardViewModel> { ViewName = "Empty", Data = model };

            if (model.PayeSchemeCount == 0)
            {
                viewModel.ViewName = "AddPAYE";                
            }
            else if (_authorizationService.IsAuthorized("EmployerFeature.CallToAction"))
            {
                _employerTeamOrchestrator.GetCallToActionViewName(viewModel);
            }

            return PartialView(viewModel);
        }

        [ChildActionOnly]
        public ActionResult Row1Panel2(AccountDashboardViewModel model)
        {
            var viewModel = new PanelViewModel<AccountDashboardViewModel> { ViewName = "Tasks", Data = model };

            if (model.PayeSchemeCount == 0)
            {
                viewModel.ViewName = "Empty";
            }

            return PartialView(viewModel);
        }

        [ChildActionOnly]
        public ActionResult Row2Panel1(AccountDashboardViewModel model)
        {
            var viewModel = new PanelViewModel<AccountDashboardViewModel> { ViewName = "Dashboard", Data = model };

            if (model.PayeSchemeCount == 0)
            {
                viewModel.ViewName = "Empty";
            }

            return PartialView(viewModel);
        }

        [ChildActionOnly]
        public ActionResult Row2Panel2(AccountDashboardViewModel model)
        {
            var viewModel = new PanelViewModel<AccountDashboardViewModel> { ViewName = "Empty", Data = model };

            return PartialView(viewModel);
        }

        [ChildActionOnly]
        public ActionResult AddPAYE(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult V2AddPAYE(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult SignAgreement(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult V2SignAgreement(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult Empty(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult Tasks(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult Dashboard(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult ProviderPermissions(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult ProviderPermissionsMultiple(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult ProviderPermissionsDenied(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult FinancialTransactions(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        public ActionResult SingleProvider(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult SavedProviders(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult AccountSettings(AccountDashboardViewModel model)
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
        public ActionResult V2CheckFunding(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult FundingComplete(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult CreateVacancy(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        private string ApplicationsDisplay(Vacancy vacancy)
        {
            return vacancy.ApplicationMethod == ApplicationMethod.ThroughExternalApplicationSite
                ? "Advertised by employer"
                : vacancy.NumberOfApplications.ToString();
        }

        [ChildActionOnly]
        public ActionResult MultipleVacancies(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult PrePayeRecruitment(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult SearchBar()
        {
            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult MostActiveLinks(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult OtherTasksPanel(AccountDashboardViewModel model)
        {
            return PartialView(model);
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
