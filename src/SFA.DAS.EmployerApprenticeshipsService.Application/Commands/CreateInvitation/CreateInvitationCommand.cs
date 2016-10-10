using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateInvitation
{
    public class CreateInvitationCommand : IAsyncRequest
    {
        public string ExternalUserId { get; set; }
        public string HashedId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Role RoleId { get; set; }
    }
}