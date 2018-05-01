using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Commands.ChangeTeamMemberRole
{
    public class ChangeTeamMemberRoleCommand : IAsyncRequest
    {
        public string HashedAccountId { get; set; }
        public string Email { get; set; }
        public short RoleId { get; set; }
        public Guid ExternalUserId { get; set; }
    }
}