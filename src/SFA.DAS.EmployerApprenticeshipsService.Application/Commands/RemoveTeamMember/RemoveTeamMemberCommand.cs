﻿using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Commands.RemoveTeamMember
{
    public class RemoveTeamMemberCommand : IAsyncRequest
    {
        public long UserId { get; set; }
        public string HashedAccountId { get; set; }
        public Guid ExternalUserId { get; set; }
    }
}