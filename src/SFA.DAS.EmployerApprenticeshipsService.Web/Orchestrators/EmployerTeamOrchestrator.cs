using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountTeamMembers;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetMember;
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

        public async Task<InvitationViewModel> GetMember(int accountId, string email)
        {
            var response = await _mediator.SendAsync(new GetMemberRequest
            {
                AccountId = accountId,
                Email = email
            });

            return new InvitationViewModel
            {
                Id = response.TeamMember.Id,
                AccountId = accountId,
                Email = response.TeamMember.Email,
                Name = "", //response.TeamMember.Name,
                Role = response.TeamMember.Role,
                Status = response.TeamMember.Status
            };
        }
    }
}