﻿using MediatR;

namespace SFA.DAS.EmployerAccounts.Commands.RemoveTeamMember
{
    public class RemoveTeamMemberCommand : IAsyncRequest
    {
        public long UserId { get; set; }
        public string HashedAccountId { get; set; }
        public string ExternalUserId { get; set; }
    }
}