using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Authentication;
using SFA.DAS.EAS.Portal.Client;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Validation;
using System.Linq;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Portal;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using System.Globalization;

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

            var hasPayeScheme = HasPayeScheme(response.Data);
            if (_authorizationService.IsAuthorized("EmployerFeature.HomePage") || !hasPayeScheme && !HasOrganisation(response.Data))
            {
                response.Data.AccountViewModel = await _portalClient.GetAccount(new GetAccountParameters
                {
                    HashedAccountId = hashedAccountId,
                    MaxNumberOfVacancies = hasPayeScheme ? 2 : 0
                });
                response.Data.ApprenticeshipAdded = response.Data.AccountViewModel?.Organisations?.FirstOrDefault()?.Cohorts?.FirstOrDefault()?.Apprenticeships?.Any() ?? false;
                response.Data.ShowMostActiveLinks = response.Data.ApprenticeshipAdded;
                response.Data.ShowSearchBar = response.Data.ApprenticeshipAdded;

                if (Guid.TryParse(reservationId, out var recentlyAddedReservationId))
                    response.Data.RecentlyAddedReservationId = recentlyAddedReservationId;

                return View("v2/Index", "_Layout_v2", response);
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
                    UserShownWizard = userShownWizard
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
        [Route("{email}/remove/")]
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
        [Route("{email}/review/")]
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

        [ChildActionOnly]
        public ActionResult Row1Panel1(AccountDashboardViewModel model)
        {
            var viewModel = new PanelViewModel<AccountDashboardViewModel> { ViewName = "CheckFunding", Data = model };
            if (model.AgreementsToSign)
            {
                viewModel.ViewName = "SignAgreement";
            }
            else if (model.ApprenticeshipAdded)
            {
                viewModel.ViewName = "ApprenticeshipDetails";
            }
            else if (model.ShowReservations) 
            {
                viewModel.ViewName = "FundingComplete";
            }
            else if(model.RecentlyAddedReservationId != null)
            {
                viewModel.ViewName = "NotCurrentlyInStorage";
            }
            else if(model.PayeSchemeCount == 0)
            {
                viewModel.ViewName = "AddPAYE";
            }
            return PartialView(viewModel);
        }

        [ChildActionOnly]
        public ActionResult Row1Panel2(AccountDashboardViewModel model)
        {
            var viewModel = new PanelViewModel<AccountDashboardViewModel> { Data = model };
            if (model.PayeSchemeCount == 0 || model.AgreementsToSign)
            {
                viewModel.ViewName = "ProviderPermissionsDenied";
            }
            else if (model.HasSingleProvider)
            {
                viewModel.ViewName = "SingleProvider";
            }
            else if (model.HasMultipleProviders)
            {
                viewModel.ViewName = "ProviderPermissionsMultiple";
            }
            else
            {
                viewModel.ViewName = "ProviderPermissions";
            }

            return PartialView(viewModel);
        }

        [ChildActionOnly]
        public ActionResult Row2Panel1(AccountDashboardViewModel model)
        {
            return PartialView(new PanelViewModel<AccountDashboardViewModel> { ViewName = "FinancialTransactions", Data = model });
        }

        [ChildActionOnly]
        public ActionResult Row2Panel2(AccountDashboardViewModel model)
        {
            var viewModel = new PanelViewModel<AccountDashboardViewModel> { ViewName = "PrePayeRecruitment", Data = model };
            if (HasPayeScheme(model))
            {
                if (model.AccountViewModel?.VacanciesRetrieved == false)
                {
                    viewModel.ViewName = "MultipleVacancies";
                }
                else
                {
                    switch (model.AccountViewModel?.GetVacancyCardinality())
                    {
                        case null:
                        case Cardinality.None:
                            viewModel.ViewName = "CreateVacancy";
                            break;
                        case Cardinality.One:
                            viewModel.ViewName = "VacancyStatus";
                            break;
                        default:
                            viewModel.ViewName = "MultipleVacancies";
                            break;
                    }
                }
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
        public ActionResult FundingComplete(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult CreateVacancy(AccountDashboardViewModel model)
        {
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult VacancyStatus(AccountDashboardViewModel model)
        {
            Vacancy vacancy = model.AccountViewModel.Vacancies.First();

            var viewModel = new VacancyStatusViewModel
            {
                VacancyTitle = vacancy.Title,
                ClosingDateText = vacancy.ClosingDate.HasValue ? vacancy.ClosingDate.Value.ToGdsFormatFull() : "-",
                ManageVacancyLinkUrl = vacancy.ManageVacancyUrl,
                ManageVacancyLinkText = "Manage vacancy",
                Reference = "VAC" + vacancy.Reference,
                Status = vacancy.Status.ToString()
            };

            switch(vacancy.Status)
            {
                case EAS.Portal.Client.Types.VacancyStatus.Closed:
                    viewModel.Applications = ApplicationsDisplay(vacancy);
                    break;

                case EAS.Portal.Client.Types.VacancyStatus.Submitted:
                    viewModel.ManageVacancyLinkText = "Preview vacancy";
                    viewModel.Status = "Pending review";
                    break;

                case EAS.Portal.Client.Types.VacancyStatus.Draft:
                    viewModel.ManageVacancyLinkText = "Edit and submit vacancy";
                    break;

                case EAS.Portal.Client.Types.VacancyStatus.Referred:
                    viewModel.ManageVacancyLinkText = "Edit and re-submit vacancy";
                    viewModel.Status = "Rejected";
                    break;

                case EAS.Portal.Client.Types.VacancyStatus.Live:
                    viewModel.Applications = ApplicationsDisplay(vacancy);
                    break;
            }

            return PartialView(viewModel);
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

        [ChildActionOnly]
        public ActionResult ApprenticeshipDetails(AccountDashboardViewModel model)
        {
            Cohort cohort = model.AccountViewModel.Organisations.FirstOrDefault()?.Cohorts?.FirstOrDefault();
            Apprenticeship apprenticeship = cohort?.Apprenticeships?.FirstOrDefault();
            
            var viewModel = new ApprenticeDetailsViewModel
            {
                ApprenticeName = $"{apprenticeship.FirstName} {apprenticeship.LastName}",
                TrainingProviderName = apprenticeship.TrainingProvider?.Name,
                CourseName = apprenticeship.CourseName,
                StartDateText = apprenticeship.StartDate?.ToGdsFormatWithoutDay(),
                EndDateText = apprenticeship.EndDate?.ToGdsFormatWithoutDay(),
                ProposedCostText = $"{apprenticeship.ProposedCost?.ToString("C0", CultureInfo.CreateSpecificCulture("en-GB"))} excluding VAT",
                IsApproved = cohort.IsApproved
            };

            return PartialView(viewModel);
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

        private bool HasPayeScheme(AccountDashboardViewModel data)
        {
            return data.PayeSchemeCount > 0;
        }

        private bool HasOrganisation(AccountDashboardViewModel data)
        {
            return data.OrgainsationCount > 0;
        }
    }
}
 