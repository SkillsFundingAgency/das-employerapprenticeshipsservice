using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ChangeTeamMemberRole
{
    public class ChangeTeamMemberRoleCommandHandler : AsyncRequestHandler<ChangeTeamMemberRoleCommand>
    {
        protected override Task HandleCore(ChangeTeamMemberRoleCommand message)
        {
            throw new System.NotImplementedException();
        }
    }
}