using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.DeleteInvitation
{
    public class DeleteInvitationCommand : IAsyncRequest
    {
        public string Email { get; set; }
        public long AccountId { get; set; }
        public string ExternalUserId { get; set; }
    }
}