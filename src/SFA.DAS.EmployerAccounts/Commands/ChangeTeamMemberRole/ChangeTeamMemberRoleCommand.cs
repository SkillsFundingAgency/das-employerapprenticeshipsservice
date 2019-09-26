﻿using MediatR;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Commands.ChangeTeamMemberRole
{
    public class ChangeTeamMemberRoleCommand : IAsyncRequest
    {
        public string HashedAccountId { get; set; }
        public string Email { get; set; }
        public Role Role{ get; set; }
        public string ExternalUserId { get; set; }
    }
}