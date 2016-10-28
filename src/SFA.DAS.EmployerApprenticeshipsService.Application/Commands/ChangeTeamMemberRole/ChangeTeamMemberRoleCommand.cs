using MediatR;

namespace SFA.DAS.EAS.Application.Commands.ChangeTeamMemberRole
{
    public class ChangeTeamMemberRoleCommand : IAsyncRequest
    {
        public string HashedId { get; set; }
        public string Email { get; set; }
        public short RoleId { get; set; }
        public string ExternalUserId { get; set; }
    }
}