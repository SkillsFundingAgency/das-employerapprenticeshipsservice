﻿using MediatR;

namespace SFA.DAS.EmployerAccounts.Commands.ChangeTeamMemberRole
{
    public class ChangeTeamMemberRoleCommand : IAsyncRequest
    {
        public string HashedAccountId { get; set; }
        public string Email { get; set; }
        public short Role { get; set; }
        public string ExternalUserId { get; set; }
    }
}