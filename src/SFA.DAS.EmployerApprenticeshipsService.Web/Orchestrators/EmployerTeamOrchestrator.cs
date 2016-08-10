using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ChangeTeamMemberRole;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateInvitation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.DeleteInvitation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.RemoveTeamMember;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ResendInvitation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountTeamMembers;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccount;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetInvitation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetMember;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
{
    public class EmployerTeamOrchestrator
    {
        private readonly IMediator _mediator;

        public EmployerTeamOrchestrator(IMediator mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            _mediator = mediator;
        }

        public async Task<OrchestratorResponse<Account>> GetAccount(long accountId, string externalUserId)
        {
            try
            {
                var response = await _mediator.SendAsync(new GetEmployerAccountQuery
                {
                    AccountId = accountId,
                    ExternalUserId = externalUserId
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
        
        public async Task<OrchestratorResponse<EmployerTeamMembersViewModel>> GetTeamMembers(long accountId, string userId)
        {
            try
            {
                var response = await _mediator.SendAsync(new GetAccountTeamMembersQuery { Id = accountId, ExternalUserId = userId });

                return new OrchestratorResponse<EmployerTeamMembersViewModel>
                {
                    Status = HttpStatusCode.OK,
                    Data = new EmployerTeamMembersViewModel
                    {
                        AccountId = accountId,
                        TeamMembers = response.TeamMembers
                    }
                };
            }
            catch (Exception ex)
            {
                return new OrchestratorResponse<EmployerTeamMembersViewModel>
                {
                    Exception = ex
                };
            }
        }

        public async Task InviteTeamMember(InviteTeamMemberViewModel model, string externalUserId)
        {
            await _mediator.SendAsync(new CreateInvitationCommand
            {
                ExternalUserId = externalUserId,
                AccountId = model.AccountId,
                Name = model.Name,
                Email = model.Email,
                RoleId = model.Role
            });
        }

        public async Task<InvitationViewModel> Review(long accountId, string email)
        {
            var response = await _mediator.SendAsync(new GetMemberRequest
            {
                AccountId = accountId,
                Email = email
            });

            return MapFrom(response.TeamMember);
        }

        public async Task<InvitationView> GetInvitation(long id)
        {
            var response = await _mediator.SendAsync(new GetInvitationRequest
            {
                Id = id
            });

            return response.Invitation;
        }

        public async Task Cancel(string email, long accountId, string externalUserId)
        {
            await _mediator.SendAsync(new DeleteInvitationCommand
            {
                Email = email,
                AccountId = accountId,
                ExternalUserId = externalUserId
            });
        }

        public async Task Resend(string email, long accountId, string externalUserId)
        {
            await _mediator.SendAsync(new ResendInvitationCommand
            {
                Email = email,
                AccountId = accountId,
                ExternalUserId = externalUserId
            });
        }

        public async Task Remove(long userId, long accountId, string externalUserId)
        {
            await _mediator.SendAsync(new RemoveTeamMemberCommand
            {
                UserId = userId,
                AccountId = accountId,
                ExternalUserId = externalUserId
            });
        }

        public async Task<TeamMember> GetTeamMember(long accountId, string email)
        {
            var response = await _mediator.SendAsync(new GetMemberRequest
            {
                AccountId = accountId,
                Email = email
            });

            return response.TeamMember;
        }

        private InvitationViewModel MapFrom(TeamMember teamMember)
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

        public async Task ChangeRole(long accountId, string email, short role, string externalUserId)
        {
            await _mediator.SendAsync(new ChangeTeamMemberRoleCommand
            {
                AccountId = accountId,
                Email = email,
                RoleId = role,
                ExternalUserId = externalUserId
            });
        }
    }
}