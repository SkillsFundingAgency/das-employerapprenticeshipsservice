using MediatR;

namespace SFA.DAS.Queries.GetUserInvitations
{
    public class GetUserInvitationsRequest : IAsyncRequest<GetUserInvitationsResponse>
    {
        public string UserId { get; set; }
    }
}