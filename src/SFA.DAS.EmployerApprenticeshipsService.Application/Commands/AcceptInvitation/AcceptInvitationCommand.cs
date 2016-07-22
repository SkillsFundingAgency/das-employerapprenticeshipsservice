using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AcceptInvitation
{
    public class AcceptInvitationCommand : IAsyncRequest
    {
        public long Id { get; set; }
    }
}