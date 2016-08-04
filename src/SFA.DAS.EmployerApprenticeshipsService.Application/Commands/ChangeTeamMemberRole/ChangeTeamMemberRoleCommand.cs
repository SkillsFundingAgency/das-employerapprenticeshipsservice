using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ChangeTeamMemberRole
{
    public class ChangeTeamMemberRoleCommand : IAsyncRequest
    {
        public long AccountId { get; set; }
        public string Email { get; set; }
        public short RoleId { get; set; }
        public string ExternalUserId { get; set; }
    }
}