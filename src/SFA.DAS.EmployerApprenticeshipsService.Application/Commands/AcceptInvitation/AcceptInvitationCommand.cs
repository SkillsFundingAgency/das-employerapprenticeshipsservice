using MediatR;

namespace SFA.DAS.EAS.Application.Commands.AcceptInvitation
{
    public class AcceptInvitationCommand : IAsyncRequest
    {
        public long Id { get; set; }
        public string ExternalUserId { get; set; }  
    }
}