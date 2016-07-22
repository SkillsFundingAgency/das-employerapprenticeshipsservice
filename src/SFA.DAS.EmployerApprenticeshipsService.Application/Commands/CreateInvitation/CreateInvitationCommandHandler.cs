using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateInvitation
{
    public class CreateInvitationCommandHandler : AsyncRequestHandler<CreateInvitationCommand>
    {
        protected override Task HandleCore(CreateInvitationCommand message)
        {
            throw new System.NotImplementedException();
        }
    }
}