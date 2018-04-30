﻿using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetUserInvitations
{
    public class GetNumberOfUserInvitationsQuery : IAsyncRequest<GetNumberOfUserInvitationsResponse>
    {
        public Guid ExternalUserId { get; set; }
    }
}