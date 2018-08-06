using MediatR;

namespace SFA.DAS.Queries.GetUserInvitations
{
    public class GetNumberOfUserInvitationsQuery : IAsyncRequest<GetNumberOfUserInvitationsResponse>
    {
        public string UserId { get; set; }
    }
}