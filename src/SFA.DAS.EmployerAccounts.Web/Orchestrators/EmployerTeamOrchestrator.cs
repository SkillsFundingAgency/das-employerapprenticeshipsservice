using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Commands.ChangeTeamMemberRole;
using SFA.DAS.EmployerAccounts.Commands.CreateInvitation;
using SFA.DAS.EmployerAccounts.Commands.DeleteInvitation;
using SFA.DAS.EmployerAccounts.Commands.RemoveTeamMember;
using SFA.DAS.EmployerAccounts.Commands.ResendInvitation;
using SFA.DAS.EmployerAccounts.Commands.UpdateShowWizard;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EmployerAccounts.Queries.GetAccountStats;
using SFA.DAS.EmployerAccounts.Queries.GetAccountTasks;
using SFA.DAS.EmployerAccounts.Queries.GetAccountTeamMembers;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using SFA.DAS.EmployerAccounts.Queries.GetInvitation;
using SFA.DAS.EmployerAccounts.Queries.GetMember;
using SFA.DAS.EmployerAccounts.Queries.GetTeamUser;
using SFA.DAS.EmployerAccounts.Queries.GetUser;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators
{
    public class EmployerTeamOrchestrator : UserVerificationOrchestratorBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentDateTime _currentDateTime;

        public EmployerTeamOrchestrator(IMediator mediator, ICurrentDateTime currentDateTime)
            : base(mediator)
        {
            _mediator = mediator;
            _currentDateTime = currentDateTime;
        }

        public async Task<OrchestratorResponse<EmployerTeamMembersViewModel>> Cancel(string email, string hashedAccountId, string externalUserId)
        {
            var response = await GetTeamMembers(hashedAccountId, externalUserId);

            if (response.Status != HttpStatusCode.OK)
            {
                return response;
            }

            try
            {
                await _mediator.SendAsync(new DeleteInvitationCommand
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
                await _mediator.SendAsync(new ChangeTeamMemberRoleCommand
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

        public async Task<OrchestratorResponse<AccountDashboardViewModel>> GetAccount(string accountId, string externalUserId)
        {
            try
            {
                var accountResponseTask = _mediator.SendAsync(new GetEmployerAccountHashedQuery
                {
                    HashedAccountId = accountId,
                    UserId = externalUserId
                });

                var userRoleResponseTask = GetUserAccountRole(accountId, externalUserId);

                var userResponseTask = _mediator.SendAsync(new GetTeamMemberQuery
                {
                    HashedAccountId = accountId,
                    TeamMemberId = externalUserId
                });

                var accountStatsResponseTask = _mediator.SendAsync(new GetAccountStatsQuery
                {
                    HashedAccountId = accountId,
                    ExternalUserId = externalUserId
                });

                var agreementsResponseTask = _mediator.SendAsync(new GetAccountEmployerAgreementsRequest
                {
                    HashedAccountId = accountId,
                    ExternalUserId = externalUserId
                });

                await Task.WhenAll(accountStatsResponseTask, userRoleResponseTask, userResponseTask, accountStatsResponseTask, agreementsResponseTask).ConfigureAwait(false);

                var accountResponse = accountResponseTask.Result;
                var userRoleResponse = userRoleResponseTask.Result;
                var userResponse = userResponseTask.Result;
                var accountStatsResponse = accountStatsResponseTask.Result;
                var agreementsResponse = agreementsResponseTask.Result;

                var tasksResponse = await _mediator.SendAsync(new GetAccountTasksQuery
                {
                    AccountId = accountResponse.Account.Id,
                    ExternalUserId = externalUserId
                });

                var requiresAgreementSigning = agreementsResponse.EmployerAgreements.Count(a => a.HasPendingAgreement);
                var tasks = tasksResponse?.Tasks.Where(t => t.ItemsDueCount > 0 && t.Type != "AgreementToSign").ToList() ?? new List<AccountTask>();
                var showWizard = userResponse.User.ShowWizard && userRoleResponse.UserRole == Role.Owner;

                var viewModel = new AccountDashboardViewModel
                {
                    Account = accountResponse.Account,
                    UserRole = userRoleResponse.UserRole,
                    HashedUserId = externalUserId,
                    UserFirstName = userResponse.User.FirstName,
                    OrgainsationCount = accountStatsResponse?.Stats?.OrganisationCount ?? 0,
                    PayeSchemeCount = accountStatsResponse?.Stats?.PayeSchemeCount ?? 0,
                    TeamMemberCount = accountStatsResponse?.Stats?.TeamMemberCount ?? 0,
                    TeamMembersInvited = accountStatsResponse?.Stats?.TeamMembersInvited ?? 0,
                    ShowWizard = showWizard,
                    ShowAcademicYearBanner = _currentDateTime.Now < new DateTime(2017, 10, 20),
                    Tasks = tasks,
                    HashedAccountId = accountId,
                    RequiresAgreementSigning = requiresAgreementSigning,
                    AgreementsToSign = requiresAgreementSigning > 0,
                    SignedAgreementCount= agreementsResponse.EmployerAgreements.Count(x => x.HasSignedAgreement)
                };

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
            catch (Exception ex)
            {
                return new OrchestratorResponse<AccountDashboardViewModel>
                {
                    Status = HttpStatusCode.InternalServerError,
                    Exception = ex
                };
            }
        }

        public async Task<OrchestratorResponse<InvitationView>> GetInvitation(string id)
        {
            var invitationResponse = await _mediator.SendAsync(new GetInvitationRequest
            {
                Id = id
            });

            var response = new OrchestratorResponse<InvitationView>
            {
                Data = invitationResponse.Invitation
            };

            return response;
        }

        public async Task<OrchestratorResponse<InviteTeamMemberViewModel>> GetNewInvitation(string hashedAccountId, string externalUserId)
        {
            var response = await GetUserAccountRole(hashedAccountId, externalUserId);

            return new OrchestratorResponse<InviteTeamMemberViewModel>
            {
                Data = new InviteTeamMemberViewModel
                {
                    HashedAccountId = hashedAccountId,
                    Role = Role.None
                },
                Status = response.UserRole.Equals(Role.Owner) ? HttpStatusCode.OK : HttpStatusCode.Unauthorized
            };
        }

        public Task<OrchestratorResponse<TeamMember>> GetActiveTeamMember(string hashedAccountId, string email, string externalUserId)
        {
            return GetTeamMember(hashedAccountId, email, externalUserId, true);
        }

        public Task<OrchestratorResponse<TeamMember>> GetTeamMemberWhetherActiveOrNot(string hashedAccountId, string email, string externalUserId)
        {
            return GetTeamMember(hashedAccountId, email, externalUserId, false);
        }

        private async Task<OrchestratorResponse<TeamMember>> GetTeamMember(string hashedAccountId, string email, string externalUserId, bool onlyIfMemberIsActive)
        {
            var userRoleResponse = await GetUserAccountRole(hashedAccountId, externalUserId);

            if (!userRoleResponse.UserRole.Equals(Role.Owner))
            {
                return new OrchestratorResponse<TeamMember>
                {
                    Status = HttpStatusCode.Unauthorized
                };
            }

            var response = await _mediator.SendAsync(new GetMemberRequest
            {
                HashedAccountId = hashedAccountId,
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
                    _mediator.SendAsync(new GetAccountTeamMembersQuery
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

        public async Task HideWizard(string hashedAccountId, string externalUserId)
        {
            await _mediator.SendAsync(new UpdateShowAccountWizardCommand
            {
                HashedAccountId = hashedAccountId,
                ExternalUserId = externalUserId,
                ShowWizard = false
            });
        }

        public async Task<OrchestratorResponse<EmployerTeamMembersViewModel>> InviteTeamMember(InviteTeamMemberViewModel model, string externalUserId)
        {
            try
            {
                await _mediator.SendAsync(new CreateInvitationCommand
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
                var userResponse = await _mediator.SendAsync(new GetUserQuery { UserId = userId });

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
                    await _mediator.SendAsync(new RemoveTeamMemberCommand
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
                        Message = $"You've removed <strong>{ userResponse.User.Email}</strong>",
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
                await _mediator.SendAsync(new ResendInvitationCommand
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
            var response = new OrchestratorResponse<InvitationViewModel>();

            var queryResponse = await _mediator.SendAsync(new GetMemberRequest
            {
                HashedAccountId = hashedAccountId,
                Email = email
            });

            response.Data = MapFrom(queryResponse.TeamMember);

            return response;
        }

        public virtual async Task<bool> UserShownWizard(string userId, string hashedAccountId)
        {
            var userResponse = await Mediator.SendAsync(new GetTeamMemberQuery { HashedAccountId = hashedAccountId, TeamMemberId = userId });
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
                ExpiryDate = teamMember.ExpiryDate
            };
        }
    }
}