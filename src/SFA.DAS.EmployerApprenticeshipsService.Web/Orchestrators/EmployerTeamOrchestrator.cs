using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateInvitation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountTeamMembers;
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
    }
}