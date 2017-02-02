using MediatR;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.Commands.CreateInvitation
{
    public class CreateInvitationCommand : IAsyncRequest
    {
        public string ExternalUserId { get; set; }
        public string HashedAccountId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Role RoleId { get; set; }
    }
}