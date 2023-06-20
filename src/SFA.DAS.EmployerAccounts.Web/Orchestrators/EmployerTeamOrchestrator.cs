using AutoMapper;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerAccounts.Commands.ChangeTeamMemberRole;
using SFA.DAS.EmployerAccounts.Commands.CreateInvitation;
using SFA.DAS.EmployerAccounts.Commands.DeleteInvitation;
using SFA.DAS.EmployerAccounts.Commands.RemoveTeamMember;
using SFA.DAS.EmployerAccounts.Commands.ResendInvitation;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.CommitmentsV2;
using SFA.DAS.EmployerAccounts.Models.Recruit;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EmployerAccounts.Queries.GetAccountStats;
using SFA.DAS.EmployerAccounts.Queries.GetAccountTasks;
using SFA.DAS.EmployerAccounts.Queries.GetAccountTeamMembers;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using SFA.DAS.EmployerAccounts.Queries.GetInvitation;
using SFA.DAS.EmployerAccounts.Queries.GetMember;
using SFA.DAS.EmployerAccounts.Queries.GetTeamUser;
using SFA.DAS.EmployerAccounts.Queries.GetUser;
using SFA.DAS.EmployerAccounts.Web.ViewComponents;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators;

public class EmployerTeamOrchestrator : UserVerificationOrchestratorBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentDateTime _currentDateTime;
    private readonly IAccountApiClient _accountApiClient;
    private readonly IMapper _mapper;
    private readonly EmployerAccountsConfiguration _configuration;
    private readonly IEncodingService _encodingService;

    public EmployerTeamOrchestrator(IMediator mediator,
        ICurrentDateTime currentDateTime,
        IAccountApiClient accountApiClient,
        IMapper mapper,
        EmployerAccountsConfiguration configuration,
        IEncodingService encodingService)
        : base(mediator)
    {
        _mediator = mediator;
        _currentDateTime = currentDateTime;
        _accountApiClient = accountApiClient;
        _mapper = mapper;
        _configuration = configuration;
        _encodingService = encodingService;
    }

    //Needed for tests	
    protected EmployerTeamOrchestrator() { }

    public async Task<OrchestratorResponse<EmployerTeamMembersViewModel>> Cancel(string email, string hashedAccountId, string externalUserId)
    {
        var response = await GetTeamMembers(hashedAccountId, externalUserId);

        if (response.Status != HttpStatusCode.OK)
        {
            return response;
        }

        try
        {
            await _mediator.Send(new DeleteInvitationCommand
            {
                Email = email,
                HashedAccountId = hashedAccountId,
                ExternalUserId = externalUserId
            });

            response = await GetTeamMembers(hashedAccountId, externalUserId);

            response.Status = HttpStatusCode.OK;

            response.FlashMessage = new FlashMessageViewModel
            {
                Headline = "Invitation cancelled",
                Message = $"You've cancelled the invitation sent to <strong>{email}</strong>",
                Severity = FlashMessageSeverityLevel.Success
            };
        }
        catch (InvalidRequestException e)
        {
            response.Status = HttpStatusCode.OK;
            response.Exception = e;
        }
        catch (UnauthorizedAccessException e)
        {
            response.Status = HttpStatusCode.OK;
            response.Exception = e;
        }

        return response;
    }

    public async Task<OrchestratorResponse<EmployerTeamMembersViewModel>> ChangeRole(string hashedId, string email, Role role, string externalUserId)
    {
        try
        {
            await _mediator.Send(new ChangeTeamMemberRoleCommand
            {
                HashedAccountId = hashedId,
                Email = email,
                Role = role,
                ExternalUserId = externalUserId
            });

            var response = await GetTeamMembers(hashedId, externalUserId);

            if (response.Status == HttpStatusCode.OK)
            {
                response.FlashMessage = new FlashMessageViewModel
                {
                    Severity = FlashMessageSeverityLevel.Success,
                    Headline = "Team member updated",
                    HiddenFlashMessageInformation = "page-team-member-role-changed",
                    Message = $"{email} can now {RoleStrings.GetRoleDescriptionToLower(role)}"
                };
            }

            return response;
        }
        catch (InvalidRequestException e)
        {
            return new OrchestratorResponse<EmployerTeamMembersViewModel>
            {
                Status = HttpStatusCode.BadRequest,
                Exception = e
            };
        }
        catch (UnauthorizedAccessException e)
        {
            return new OrchestratorResponse<EmployerTeamMembersViewModel>
            {
                Status = HttpStatusCode.Unauthorized,
                Exception = e
            };
        }
    }

    public virtual async Task<OrchestratorResponse<AccountDashboardViewModel>> GetAccount(string hashedAccountId, string externalUserId)
    {
        var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);

        try
        {
            var accountDetailViewModel = await _accountApiClient.GetAccount(hashedAccountId);

            var accountResponse = await _mediator.Send(new GetEmployerAccountByIdQuery
            {
                AccountId = accountId,
                UserId = externalUserId
            });

            var userRoleResponse = await GetUserAccountRole(accountId, externalUserId);

            var userResponse = await _mediator.Send(new GetTeamMemberQuery
            {
                AccountId = accountId,
                TeamMemberId = externalUserId
            });

            var accountStatsResponse = await _mediator.Send(new GetAccountStatsQuery
            {
                AccountId = accountId,
                ExternalUserId = externalUserId
            });

            var agreementsResponse = await _mediator.Send(new GetAccountEmployerAgreementsRequest
            {
                AccountId = accountId,
                ExternalUserId = externalUserId
            });

            var userQueryResponse = await _mediator.Send(new GetUserByRefQuery
            {
                UserRef = externalUserId
            });

            var apprenticeshipEmployerType = (ApprenticeshipEmployerType)Enum.Parse(typeof(ApprenticeshipEmployerType), accountDetailViewModel.ApprenticeshipEmployerType, true);

            var tasksResponse = await _mediator.Send(new GetAccountTasksQuery
            {
                AccountId = accountResponse.Account.Id,
                ExternalUserId = externalUserId,
                ApprenticeshipEmployerType = apprenticeshipEmployerType
            });

            var pendingAgreements = agreementsResponse.EmployerAgreements.Where(a => a.HasPendingAgreement && !a.HasSignedAgreement).Select(a => new PendingAgreementsViewModel { HashedAgreementId = a.Pending.HashedAgreementId }).ToList();
            var tasks = tasksResponse?.Tasks.Where(t => t.ItemsDueCount > 0 && t.Type != "AgreementToSign").ToList() ?? new List<AccountTask>();
            var showWizard = userResponse.User.ShowWizard && userRoleResponse.UserRole == Role.Owner;

            var viewModel = new AccountDashboardViewModel
            {
                HashedAccountId = hashedAccountId,
                Account = accountResponse.Account,
                SingleAccountLegalEntityId = agreementsResponse.EmployerAgreements.Count == 1 ? agreementsResponse.EmployerAgreements.First().LegalEntity.AccountLegalEntityPublicHashedId : null,
                UserRole = userRoleResponse.UserRole,
                HashedUserId = externalUserId,
                UserFirstName = userResponse.User.FirstName,
                OrganisationCount = accountStatsResponse?.Stats?.OrganisationCount ?? 0,
                PayeSchemeCount = accountStatsResponse?.Stats?.PayeSchemeCount ?? 0,
                TeamMemberCount = accountStatsResponse?.Stats?.TeamMemberCount ?? 0,
                TeamMembersInvited = accountStatsResponse?.Stats?.TeamMembersInvited ?? 0,
                ShowWizard = showWizard,
                //TODO: Delete this?
                ShowAcademicYearBanner = _currentDateTime.Now < new DateTime(2017, 10, 20),
                Tasks = tasks,
                RequiresAgreementSigning = pendingAgreements.Count,
                SignedAgreementCount = agreementsResponse.EmployerAgreements.Count(x => x.HasSignedAgreement),
                PendingAgreements = pendingAgreements,
                ApprenticeshipEmployerType = apprenticeshipEmployerType,
                AgreementInfo = _mapper.Map<AccountDetailViewModel, AgreementInfoViewModel>(accountDetailViewModel),
                TermAndConditionsAcceptedOn = userQueryResponse.User.TermAndConditionsAcceptedOn,
                LastTermsAndConditionsUpdate = _configuration.LastTermsAndConditionsUpdate
            };

            //note: ApprenticeshipEmployerType is already returned by GetEmployerAccountHashedQuery, but we need to transition to calling the api instead.
            // we could blat over the existing flag, but it's much nicer to store the enum (as above) rather than a byte!
            //viewModel.Account.ApprenticeshipEmployerType = (byte) ((ApprenticeshipEmployerType) Enum.Parse(typeof(ApprenticeshipEmployerType), apiGetAccountTask.Result.ApprenticeshipEmployerType, true));

            return new OrchestratorResponse<AccountDashboardViewModel>
            {
                Status = HttpStatusCode.OK,
                Data = viewModel
            };
        }
        catch (UnauthorizedAccessException ex)
        {
            return new OrchestratorResponse<AccountDashboardViewModel>
            {
                Status = HttpStatusCode.Unauthorized,
                Exception = ex
            };
        }
        catch (System.Net.Http.HttpRequestException ex)
        {
            return new OrchestratorResponse<AccountDashboardViewModel>
            {
                Status = HttpStatusCode.InternalServerError,
                Exception = new ResourceNotFoundException($"An error occured whilst trying to retrieve account: {accountId}", ex)
            };
        }
        catch (Exception ex)
        {
            return new OrchestratorResponse<AccountDashboardViewModel>
            {
                Status = HttpStatusCode.InternalServerError,
                Exception = ex
            };
        }
    }

    public async Task<OrchestratorResponse<InvitationView>> GetInvitation(string invitationId)
    {
        var decodedId = _encodingService.Decode(invitationId, EncodingType.AccountId);
        var invitationResponse = await _mediator.Send(new GetInvitationRequest
        {
            Id = decodedId
        });

        var response = new OrchestratorResponse<InvitationView>
        {
            Data = invitationResponse.Invitation
        };

        return response;
    }

    public async Task<OrchestratorResponse<InviteTeamMemberViewModel>> GetNewInvitation(string hashedAccountId, string externalUserId)
    {
        var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);
        var response = await GetUserAccountRole(accountId, externalUserId);

        return new OrchestratorResponse<InviteTeamMemberViewModel>
        {
            Data = new InviteTeamMemberViewModel
            {
                Role = Role.None
            },
            Status = response.UserRole.Equals(Role.Owner) ? HttpStatusCode.OK : HttpStatusCode.Unauthorized
        };
    }

    public Task<OrchestratorResponse<TeamMember>> GetActiveTeamMember(long accountId, string email, string externalUserId)
    {
        return GetTeamMember(accountId, email, externalUserId, true);
    }

    public virtual Task<OrchestratorResponse<TeamMember>> GetTeamMemberWhetherActiveOrNot(string hashedAccountId, string email, string externalUserId)
    {
        var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);

        return GetTeamMemberWhetherActiveOrNot(accountId, email, externalUserId);
    }

    public Task<OrchestratorResponse<TeamMember>> GetTeamMemberWhetherActiveOrNot(long accountId, string email, string externalUserId)
    {
        return GetTeamMember(accountId, email, externalUserId, false);
    }

    private async Task<OrchestratorResponse<TeamMember>> GetTeamMember(long accountId, string email, string externalUserId, bool onlyIfMemberIsActive)
    {
        var userRoleResponse = await GetUserAccountRole(accountId, externalUserId);

        if (!userRoleResponse.UserRole.Equals(Role.Owner))
        {
            return new OrchestratorResponse<TeamMember>
            {
                Status = HttpStatusCode.Unauthorized
            };
        }

        var response = await _mediator.Send(new GetMemberRequest
        {
            AccountId = accountId,
            Email = email,
            OnlyIfMemberIsActive = onlyIfMemberIsActive
        });

        return new OrchestratorResponse<TeamMember>
        {
            Status = response.TeamMember.AccountId == 0 ? HttpStatusCode.NotFound : HttpStatusCode.OK,
            Data = response.TeamMember
        };
    }

    public async Task<OrchestratorResponse<EmployerTeamMembersViewModel>> GetTeamMembers(string hashedId, string userId)
    {
        try
        {
            var response = await
                _mediator.Send(new GetAccountTeamMembersQuery
                {
                    HashedAccountId = hashedId,
                    ExternalUserId = userId
                });

            return new OrchestratorResponse<EmployerTeamMembersViewModel>
            {
                Status = HttpStatusCode.OK,
                Data = new EmployerTeamMembersViewModel
                {
                    HashedAccountId = hashedId,
                    TeamMembers = response.TeamMembers
                }
            };
        }
        catch (InvalidRequestException ex)
        {
            return new OrchestratorResponse<EmployerTeamMembersViewModel>
            {
                Status = HttpStatusCode.BadRequest,
                Exception = ex
            };
        }
        catch (UnauthorizedAccessException ex)
        {
            return new OrchestratorResponse<EmployerTeamMembersViewModel>
            {
                Status = HttpStatusCode.Unauthorized,
                Exception = ex
            };
        }
    }

    public async Task<OrchestratorResponse<EmployerTeamMembersViewModel>> InviteTeamMember(InviteTeamMemberViewModel model, string externalUserId)
    {
        try
        {
            await _mediator.Send(new CreateInvitationCommand
            {
                ExternalUserId = externalUserId,
                HashedAccountId = model.HashedAccountId,
                NameOfPersonBeingInvited = model.Name,
                EmailOfPersonBeingInvited = model.Email,
                RoleOfPersonBeingInvited = model.Role
            });
        }
        catch (InvalidRequestException e)
        {
            return new OrchestratorResponse<EmployerTeamMembersViewModel>
            {
                Status = HttpStatusCode.BadRequest,
                FlashMessage = new FlashMessageViewModel
                {
                    Headline = "Errors to fix",
                    Message = "Check the following details:",
                    ErrorMessages = e.ErrorMessages,
                    Severity = FlashMessageSeverityLevel.Error
                },
                Exception = e
            };
        }
        catch (UnauthorizedAccessException ex)
        {
            return new OrchestratorResponse<EmployerTeamMembersViewModel>
            {
                Status = HttpStatusCode.Unauthorized,
                Exception = ex
            };
        }

        return new OrchestratorResponse<EmployerTeamMembersViewModel>();
    }

    public async Task<OrchestratorResponse<EmployerTeamMembersViewModel>> Remove(long userId, string accountId, string externalUserId)
    {
        var response = await GetTeamMembers(accountId, externalUserId);

        if (response.Status != HttpStatusCode.OK)
        {
            return response;
        }

        try
        {
            var userResponse = await _mediator.Send(new GetUserQuery { UserId = userId });

            if (userResponse?.User == null)
            {
                response.Status = HttpStatusCode.NotFound;
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = "Could not find user",
                    Message = "The user being removed from the team could not be found",
                    Severity = FlashMessageSeverityLevel.Error
                };
            }
            else
            {
                await _mediator.Send(new RemoveTeamMemberCommand
                {
                    UserId = userId,
                    UserRef = userResponse.User.Ref,
                    HashedAccountId = accountId,
                    ExternalUserId = externalUserId
                });

                //Update the team members list after the user has been removed
                response = await GetTeamMembers(accountId, externalUserId);

                if (response.Status != HttpStatusCode.OK)
                    return response;

                response.Status = HttpStatusCode.OK;

                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = "Team member removed",
                    Message = $"You've removed <strong>{userResponse.User.Email}</strong>",
                    HiddenFlashMessageInformation = "page-team-member-deleted",
                    Severity = FlashMessageSeverityLevel.Success
                };
            }
        }
        catch (InvalidRequestException e)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.FlashMessage = new FlashMessageViewModel
            {
                Headline = "Errors to fix",
                Message = "Check the following details:",
                ErrorMessages = e.ErrorMessages,
                Severity = FlashMessageSeverityLevel.Error
            };
            response.Exception = e;
            response.FlashMessage = new FlashMessageViewModel
            {
                Headline = "Errors to fix",
                Message = "Check the following details:",
                ErrorMessages = e.ErrorMessages,
                Severity = FlashMessageSeverityLevel.Error
            };
        }
        catch (UnauthorizedAccessException e)
        {
            response.Status = HttpStatusCode.Unauthorized;
            response.Exception = e;
        }

        return response;
    }

    public async Task<OrchestratorResponse<EmployerTeamMembersViewModel>> Resend(string email, string hashedId, string externalUserId, string name)
    {
        var response = await GetTeamMembers(hashedId, externalUserId);

        if (response.Status != HttpStatusCode.OK)
        {
            return response;
        }

        try
        {
            await _mediator.Send(new ResendInvitationCommand
            {
                Email = email,
                AccountId = hashedId,
                FirstName = name,
                ExternalUserId = externalUserId
            });

            //Refresh team members view
            response = await GetTeamMembers(hashedId, externalUserId);
            response.Status = HttpStatusCode.OK;
            response.FlashMessage = new FlashMessageViewModel
            {
                Severity = FlashMessageSeverityLevel.Success,
                Headline = $"Invitation resent",
                Message = $"You've resent an invitation to <strong>{email}</strong>"
            };
        }
        catch (InvalidRequestException e)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.Exception = e;
        }
        catch (UnauthorizedAccessException e)
        {
            response.Status = HttpStatusCode.Unauthorized;
            response.Exception = e;
        }

        return response;
    }

    public async Task<OrchestratorResponse<InvitationViewModel>> Review(string hashedAccountId, string email)
    {
        var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);
        var response = new OrchestratorResponse<InvitationViewModel>();

        var queryResponse = await _mediator.Send(new GetMemberRequest
        {
            AccountId = accountId,
            Email = email
        });

        response.Data = MapFrom(queryResponse.TeamMember);

        return response;
    }

    public virtual async Task<bool> UserShownWizard(string userId, string hashedAccountId)
    {
        var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);
        var userResponse = await Mediator.Send(new GetTeamMemberQuery { AccountId = accountId, TeamMemberId = userId });
        return userResponse.User.ShowWizard && userResponse.User.Role == Role.Owner;
    }

    private static InvitationViewModel MapFrom(TeamMember teamMember)
    {
        return new InvitationViewModel
        {
            IsUser = teamMember.IsUser,
            Id = teamMember.Id,
            AccountId = teamMember.AccountId,
            Email = teamMember.Email,
            Name = teamMember.Name,
            Role = teamMember.Role,
            Status = teamMember.Status,
            ExpiryDate = teamMember.ExpiryDate,
            HashedAccountId = teamMember.HashedAccountId
        };
    }

    public virtual async Task<OrchestratorResponse<AccountSummaryViewModel>> GetAccountSummary(string hashedAccountId, string externalUserId)
    {
        var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);

        try
        {
            var accountResponse = await _mediator.Send(new GetEmployerAccountByIdQuery
            {
                AccountId = accountId,
                UserId = externalUserId
            });

            var viewModel = new AccountSummaryViewModel
            {
                Account = accountResponse.Account
            };

            return new OrchestratorResponse<AccountSummaryViewModel>
            {
                Status = HttpStatusCode.OK,
                Data = viewModel
            };
        }
        catch (InvalidRequestException ex)
        {
            return new OrchestratorResponse<AccountSummaryViewModel>
            {
                Status = HttpStatusCode.BadRequest,
                Data = new AccountSummaryViewModel(),
                Exception = ex
            };
        }
        catch (UnauthorizedAccessException)
        {
            return new OrchestratorResponse<AccountSummaryViewModel>
            {
                Status = HttpStatusCode.Unauthorized
            };
        }
    }

    public void GetCallToActionViewName(PanelViewModel<AccountDashboardViewModel> viewModel)
    {
        var rules = new Dictionary<int, EvaluateCallToActionRuleDelegate>
        {
            { 100, EvaluateSignAgreementCallToActionRule },
            { 101, vm => viewModel.Data.CallToActionViewModel == null }
        };

        if (viewModel.Data.ApprenticeshipEmployerType != ApprenticeshipEmployerType.Levy)
        {
            rules.Add(120, EvaluateSingleApprenticeshipCallToActionRule);
            rules.Add(121, EvaluateSingleApprenticeshipDraftStatusCallToActionRule);
            rules.Add(122, EvaluateSingleApprenticeshipsWithTrainingProviderStatusCallToActionRule);
            rules.Add(123, EvaluateSingleApprenticeshipsWithReadyToReviewStatusCallToActionRule);
            rules.Add(124, EvaluateContinueSetupForSingleApprenticeshipByProviderCallToActionRule);
            rules.Add(200, EvaluateSingleReservationCallToActionRule);
            rules.Add(201, EvaluateHasReservationsCallToActionRule);

            rules.Add(150, EvaluateDraftVacancyCallToActionRule);
            rules.Add(151, EvaluatePendingReviewVacancyCallToActionRule);
            rules.Add(152, EvaluateLiveVacancyCallToActionRule);
            rules.Add(153, EvaluateRejectedVacancyCallToActionRule);
            rules.Add(154, EvaluateClosedVacancyCallToActionRule);
        }

        foreach (var callToActionRuleFunc in rules.OrderBy(r => r.Key))
        {
            if (callToActionRuleFunc.Value(viewModel))
                return;
        }
    }

    private delegate bool EvaluateCallToActionRuleDelegate(PanelViewModel<AccountDashboardViewModel> viewModel);

    private bool EvaluateDraftVacancyCallToActionRule(PanelViewModel<AccountDashboardViewModel> viewModel)
    {
        if (viewModel.Data.CallToActionViewModel.VacanciesViewModel.VacancyCount != 1 ||
            viewModel.Data.ApprenticeshipEmployerType == ApprenticeshipEmployerType.Levy)
        {
            return false;
        }

        if (viewModel.Data.CallToActionViewModel.VacanciesViewModel.Vacancies.First().Status.Equals(VacancyStatus.Draft))
        {
            viewModel.ComponentName = ComponentConstants.VacancyDraft;
            viewModel.PanelType = PanelType.Summary;
            viewModel.Data.HideTasksBar = true;
            return true;
        }

        return false;
    }

    private static bool EvaluatePendingReviewVacancyCallToActionRule(PanelViewModel<AccountDashboardViewModel> viewModel)
    {
        if (viewModel.Data.CallToActionViewModel.VacanciesViewModel.VacancyCount != 1 ||
            viewModel.Data.ApprenticeshipEmployerType == ApprenticeshipEmployerType.Levy)
        {
            return false;
        }

        if (viewModel.Data.CallToActionViewModel.VacanciesViewModel.Vacancies.First().Status.Equals(VacancyStatus.Submitted))
        {
            viewModel.ComponentName = ComponentConstants.VacancyPendingReview;
            viewModel.PanelType = PanelType.Summary;
            viewModel.Data.HideTasksBar = true;
            return true;
        }

        return false;
    }

    private static bool EvaluateLiveVacancyCallToActionRule(PanelViewModel<AccountDashboardViewModel> viewModel)
    {
        if (viewModel.Data.CallToActionViewModel.VacanciesViewModel.VacancyCount != 1 ||
            viewModel.Data.ApprenticeshipEmployerType == ApprenticeshipEmployerType.Levy)
        {
            return false;
        }

        if (viewModel.Data.CallToActionViewModel.VacanciesViewModel.Vacancies.First().Status.Equals(VacancyStatus.Live))
        {
            viewModel.ComponentName = ComponentConstants.VacancyLive;
            viewModel.PanelType = PanelType.Summary;
            viewModel.Data.HideTasksBar = true;
            return true;
        }

        return false;
    }
    private static bool EvaluateClosedVacancyCallToActionRule(PanelViewModel<AccountDashboardViewModel> viewModel)
    {
        if (viewModel.Data.CallToActionViewModel.VacanciesViewModel.VacancyCount != 1 ||
            viewModel.Data.ApprenticeshipEmployerType == ApprenticeshipEmployerType.Levy)
        {
            return false;
        }

        if (viewModel.Data.CallToActionViewModel.VacanciesViewModel.Vacancies.First().Status.Equals(VacancyStatus.Closed))
        {
            viewModel.ComponentName = ComponentConstants.VacancyClosed;
            viewModel.PanelType = PanelType.Summary;
            viewModel.Data.HideTasksBar = true;
            return true;
        }

        return false;
    }

    private static bool EvaluateRejectedVacancyCallToActionRule(PanelViewModel<AccountDashboardViewModel> viewModel)
    {
        if (viewModel.Data.CallToActionViewModel.VacanciesViewModel.VacancyCount != 1 ||
            viewModel.Data.ApprenticeshipEmployerType == ApprenticeshipEmployerType.Levy)
        {
            return false;
        }

        if (viewModel.Data.CallToActionViewModel.VacanciesViewModel.Vacancies.First().Status.Equals(VacancyStatus.Referred))
        {
            viewModel.ComponentName = ComponentConstants.VacancyRejected;
            viewModel.PanelType = PanelType.Summary;
            viewModel.Data.HideTasksBar = true;
            return true;
        }

        return false;
    }

    private static bool EvaluateSignAgreementCallToActionRule(PanelViewModel<AccountDashboardViewModel> viewModel)
    {
        if (viewModel.Data.PendingAgreements?.Count > 0)
        {
            viewModel.ComponentName = ComponentConstants.SignAgreement;
            return true;
        }

        return false;
    }

    private static bool EvaluateSingleReservationCallToActionRule(PanelViewModel<AccountDashboardViewModel> viewModel)
    {
        if (viewModel.Data.CallToActionViewModel.ReservationsCount == 1
            && viewModel.Data.CallToActionViewModel.PendingReservationsCount == 1)
        {
            viewModel.ComponentName = ComponentConstants.ContinueSetupForSingleReservation;
            viewModel.PanelType = PanelType.Summary;
            viewModel.Data.HideTasksBar = true;
            return true;
        }

        return false;
    }

    private static bool EvaluateHasReservationsCallToActionRule(PanelViewModel<AccountDashboardViewModel> viewModel)
    {
        if (!viewModel.Data.CallToActionViewModel.HasReservations
            && viewModel.Data.CallToActionViewModel.CohortsCount == 0
            && viewModel.Data.CallToActionViewModel.VacanciesViewModel.VacancyCount == 0)
        {
            viewModel.ComponentName = ComponentConstants.CheckFunding;
            viewModel.PanelType = PanelType.Action;
            viewModel.Data.HideTasksBar = true;
            return true;
        }

        return false;
    }

    private static bool EvaluateSingleApprenticeshipCallToActionRule(PanelViewModel<AccountDashboardViewModel> viewModel)
    {
        if (viewModel.Data.CallToActionViewModel.ReservationsCount == 1
            && viewModel.Data.CallToActionViewModel?.ApprenticeshipsCount == 1)
        {
            viewModel.ComponentName = ComponentConstants.SingleApprenticeshipApproved;
            viewModel.PanelType = PanelType.Summary;
            viewModel.Data.HideTasksBar = true;
            return true;
        }

        return false;
    }

    private static bool EvaluateSingleApprenticeshipDraftStatusCallToActionRule(PanelViewModel<AccountDashboardViewModel> viewModel)
    {
        if (viewModel.Data.CallToActionViewModel.ReservationsCount == 1
            && viewModel.Data.CallToActionViewModel.CohortsCount == 1
            && viewModel.Data.CallToActionViewModel.Cohorts.Single() != null
            && viewModel.Data.CallToActionViewModel.ApprenticeshipsCount == 0
            && viewModel.Data.CallToActionViewModel.Cohorts?.Single().CohortApprenticeshipsCount == 1
            && viewModel.Data.CallToActionViewModel.Cohorts.Single().Apprenticeships.Single().HasSingleDraftApprenticeship.Equals(true)
            && viewModel.Data.CallToActionViewModel.Cohorts.Single().CohortStatus.Equals(CohortStatus.Draft))
        {
            viewModel.ComponentName = ComponentConstants.SingleApprenticeshipContinueSetup;
            viewModel.PanelType = PanelType.Summary;
            viewModel.Data.HideTasksBar = true;
            return true;
        }

        return false;
    }

    private static bool EvaluateSingleApprenticeshipsWithTrainingProviderStatusCallToActionRule(PanelViewModel<AccountDashboardViewModel> viewModel)
    {
        if (viewModel.Data.CallToActionViewModel.ReservationsCount == 1
            && viewModel.Data.CallToActionViewModel.CohortsCount == 1
            && viewModel.Data.CallToActionViewModel.Cohorts.Single() != null
            && viewModel.Data.CallToActionViewModel.ApprenticeshipsCount == 0
            && viewModel.Data.CallToActionViewModel.Cohorts?.Single().CohortApprenticeshipsCount == 1
            && viewModel.Data.CallToActionViewModel.Cohorts.Single().Apprenticeships.Single().HasSingleDraftApprenticeship.Equals(true)
            && viewModel.Data.CallToActionViewModel.Cohorts.Single().CohortStatus.Equals(CohortStatus.WithTrainingProvider))
        {
            viewModel.ComponentName = ComponentConstants.SingleApprenticeshipWithTrainingProvider;
            viewModel.PanelType = PanelType.Summary;
            viewModel.Data.HideTasksBar = true;
            return true;
        }
        return false;
    }

    private static bool EvaluateContinueSetupForSingleApprenticeshipByProviderCallToActionRule(PanelViewModel<AccountDashboardViewModel> viewModel)
    {
        if (viewModel.Data.CallToActionViewModel.ReservationsCount == 1
            && viewModel.Data.CallToActionViewModel.CohortsCount == 1
            && viewModel.Data.CallToActionViewModel.Cohorts.Single() != null
            && viewModel.Data.CallToActionViewModel.ApprenticeshipsCount == 0
            && viewModel.Data.CallToActionViewModel.Cohorts.Single().NumberOfDraftApprentices == 1
            && viewModel.Data.CallToActionViewModel.Cohorts.Single().CohortStatus.Equals(CohortStatus.WithTrainingProvider))
        {
            viewModel.ComponentName = ComponentConstants.SingleApprenticeshipContinueWithProvider;
            viewModel.PanelType = PanelType.Summary;
            viewModel.Data.HideTasksBar = true;
            return true;
        }

        return false;
    }

    private static bool EvaluateSingleApprenticeshipsWithReadyToReviewStatusCallToActionRule(PanelViewModel<AccountDashboardViewModel> viewModel)
    {
        if (viewModel.Data.CallToActionViewModel.ReservationsCount == 1
            && viewModel.Data.CallToActionViewModel.CohortsCount == 1
            && viewModel.Data.CallToActionViewModel.Cohorts.Single() != null
            && viewModel.Data.CallToActionViewModel.ApprenticeshipsCount == 0
            && viewModel.Data.CallToActionViewModel.Cohorts?.Single().CohortApprenticeshipsCount == 1
            && viewModel.Data.CallToActionViewModel.Cohorts.Single().Apprenticeships.Single().HasSingleDraftApprenticeship.Equals(true)
            && viewModel.Data.CallToActionViewModel.Cohorts.Single().CohortStatus.Equals(CohortStatus.Review))
        {
            viewModel.ComponentName = ComponentConstants.SingleApprenticeshipReadyForReview;
            viewModel.PanelType = PanelType.Summary;
            viewModel.Data.HideTasksBar = true;
            return true;
        }

        return false;
    }
}