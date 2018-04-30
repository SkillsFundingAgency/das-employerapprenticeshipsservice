using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetUserInvitations
{
    public class GetUserInvitationsRequest : IAsyncRequest<GetUserInvitationsResponse>
    {
        public Guid ExternalUserId { get; set; }
    }
}