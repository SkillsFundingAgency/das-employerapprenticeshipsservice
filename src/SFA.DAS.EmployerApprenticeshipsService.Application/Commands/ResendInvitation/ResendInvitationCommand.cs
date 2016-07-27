using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ResendInvitation
{
    public class ResendInvitationCommand : IAsyncRequest
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string ExternalUserId { get; set; }
    }
}