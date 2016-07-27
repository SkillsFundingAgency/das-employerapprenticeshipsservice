using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateInvitation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.DeleteInvitation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountTeamMembers;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetInvitation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetMember;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
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
        
        public async Task<EmployerTeamMembersViewModel> GetTeamMembers(int accountId, string userId)
        {
            var accountTeamMemberReponse = await _mediator.SendAsync(new GetAccountTeamMembersQuery { Id = accountId, UserId = userId });
            return new EmployerTeamMembersViewModel { TeamMembers = accountTeamMemberReponse.TeamMembers, AccountId = accountId };
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

        public async Task Cancel(long id, long accountId, string externalUserId)
        {
            await _mediator.SendAsync(new DeleteInvitationCommand
            {
                Id = id,
                AccountId = accountId,
                ExternalUserId = externalUserId
            });
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
    }
}