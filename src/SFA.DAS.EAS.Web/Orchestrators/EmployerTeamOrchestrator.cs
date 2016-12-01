using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Commands.ChangeTeamMemberRole;
using SFA.DAS.EAS.Application.Commands.CreateInvitation;
using SFA.DAS.EAS.Application.Commands.DeleteInvitation;
using SFA.DAS.EAS.Application.Commands.RemoveTeamMember;
using SFA.DAS.EAS.Application.Commands.ResendInvitation;
using SFA.DAS.EAS.Application.Queries.GetAccountTeamMembers;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Application.Queries.GetInvitation;
using SFA.DAS.EAS.Application.Queries.GetMember;
using SFA.DAS.EAS.Application.Queries.GetUser;
using SFA.DAS.EAS.Application.Queries.GetUserAccountRole;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class EmployerTeamOrchestrator: UserVerificationOrchestratorBase
    {
        private readonly IMediator _mediator;

        public EmployerTeamOrchestrator(IMediator mediator):base(mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            _mediator = mediator;
        }

        public async Task<OrchestratorResponse<Account>> GetAccount(
            string accountId, string externalUserId)
        {
            try
            {
                var response = await _mediator.SendAsync(new GetEmployerAccountHashedQuery
                {
                    HashedId = accountId,
                    UserId = externalUserId
                });

                return new OrchestratorResponse<Account>
                {
                    Status = HttpStatusCode.OK,
                    Data = response.Account
                };
            }
            catch (Exception ex)
            {
                return new OrchestratorResponse<Account>
                {
                    Status = HttpStatusCode.Unauthorized,
                    Exception = ex
                };
            }
        }

        public async Task<OrchestratorResponse<EmployerTeamMembersViewModel>> GetTeamMembers(
            string hashedId, string userId)
        {
            try
            {
                var response =
                    await
                        _mediator.SendAsync(new GetAccountTeamMembersQuery
                        {
                            HashedId = hashedId,
                            ExternalUserId = userId
                        });

                return new OrchestratorResponse<EmployerTeamMembersViewModel>
                {
                    Status = HttpStatusCode.OK,
                    Data = new EmployerTeamMembersViewModel
                    {
                        HashedId = hashedId,
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

        public async Task<OrchestratorResponse<EmployerTeamMembersViewModel>> InviteTeamMember(
            InviteTeamMemberViewModel model, string externalUserId)
        {
            try
            {
                await _mediator.SendAsync(new CreateInvitationCommand
                {
                    ExternalUserId = externalUserId,
                    HashedId = model.AccountHashedId,
                    Name = model.Name,
                    Email = model.Email,
                    RoleId = model.Role
                });
            }
            catch (InvalidRequestException e)
            {
                return new OrchestratorResponse<EmployerTeamMembersViewModel>
                {
                    Status = HttpStatusCode.BadRequest,
                    FlashMessage =  new FlashMessageViewModel
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

            var response = await GetTeamMembers(model.AccountHashedId, externalUserId);

            if (response.Status == HttpStatusCode.OK)
            {
                response.FlashMessage = new FlashMessageViewModel
                {
                    Severity = FlashMessageSeverityLevel.Success,
                    Headline = "Invitation sent",
                    Message = $"You've sent an invitation to <strong>{model.Email}</strong>"
                };
            }

            return response;
        }

        public async Task<OrchestratorResponse<InvitationViewModel>> Review(
            string accountId, string email)
        {
            var response = new OrchestratorResponse<InvitationViewModel>();

            var queryResponse = await _mediator.SendAsync(new GetMemberRequest
            {
                HashedId = accountId,
                Email = email
            });

            response.Data = MapFrom(queryResponse.TeamMember);

            return response;
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

        public async Task<OrchestratorResponse<EmployerTeamMembersViewModel>> Cancel(
            string email, string hashedId, string externalUserId)
        {
            var response = await GetTeamMembers(hashedId, externalUserId);

            if (response.Status != HttpStatusCode.OK)
                return response;

            try
            {
                await _mediator.SendAsync(new DeleteInvitationCommand
                {
                    Email = email,
                    HashedId = hashedId,
                    ExternalUserId = externalUserId
                });

                response = await GetTeamMembers(hashedId, externalUserId);

                response.Status = HttpStatusCode.OK;
                response.FlashMessage = new FlashMessageViewModel()
                {
                    Headline = "Invitation cancelled",
                    Message = $"You've cancelled the invitation sent to <b>{email}</strong>",
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

        public async Task<OrchestratorResponse<EmployerTeamMembersViewModel>> Resend(
            string email, string hashedId, string externalUserId)
        {
            var response = await GetTeamMembers(hashedId, externalUserId);

            if (response.Status != HttpStatusCode.OK)
                return response;
            
            try
            {
                await _mediator.SendAsync(new ResendInvitationCommand
                {
                    Email = email,
                    AccountId = hashedId,
                    ExternalUserId = externalUserId
                });

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

        public async Task<OrchestratorResponse<EmployerTeamMembersViewModel>> Remove(
            long userId, string accountId, string externalUserId)
        {
            var response = await GetTeamMembers(accountId, externalUserId);

            if (response.Status != HttpStatusCode.OK)
                return response;

            try
            {
                var userResponse = await _mediator.SendAsync(new GetUserQuery {UserId = userId});

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
                        HashedId = accountId,
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

        public async Task<OrchestratorResponse<TeamMember>> GetTeamMember(
            string hashedAccountId, string email, string externalUserId)
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
                HashedId = hashedAccountId,
                Email = email
            });

            return new OrchestratorResponse<TeamMember>
            {
                Data = response.TeamMember
            };
        }

        public async Task<OrchestratorResponse<EmployerTeamMembersViewModel>> ChangeRole(
            string hashedId, string email, short role, string externalUserId)
        {
            try
            {
                await _mediator.SendAsync(new ChangeTeamMemberRoleCommand
                {
                    HashedId = hashedId,
                    Email = email,
                    RoleId = role,
                    ExternalUserId = externalUserId
                });

                var response = await GetTeamMembers(hashedId, externalUserId);

                if (response.Status == HttpStatusCode.OK)
                {
                    response.FlashMessage = new FlashMessageViewModel()
                    {
                        Severity = FlashMessageSeverityLevel.Success,
                        Headline = "Team member updated",
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

        public async Task<OrchestratorResponse<InviteTeamMemberViewModel>> GetNewInvitation(
            string hashedAccountId, string externalUserId)
        {

            var response = await GetUserAccountRole(hashedAccountId, externalUserId);

            return new OrchestratorResponse<InviteTeamMemberViewModel>
            {
                Data = new InviteTeamMemberViewModel
                {
                    AccountHashedId = hashedAccountId,
                    Role = Role.None
                },
                Status = response.UserRole.Equals(Role.Owner) ? HttpStatusCode.OK : HttpStatusCode.Unauthorized
            };


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