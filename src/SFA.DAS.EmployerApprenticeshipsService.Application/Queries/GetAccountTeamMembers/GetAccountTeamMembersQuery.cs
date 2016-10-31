﻿using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetAccountTeamMembers
{
    public class GetAccountTeamMembersQuery : IAsyncRequest<GetAccountTeamMembersResponse>
    {
        public string HashedId { get; set; }
        public string ExternalUserId { get; set; }
    }
}


