using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AcceptInvitation
{
    public class AcceptInvitationCommand : IAsyncRequest
    {
        public long Id { get; set; }
        public string ExternalUserId { get; set; }  
    }
}