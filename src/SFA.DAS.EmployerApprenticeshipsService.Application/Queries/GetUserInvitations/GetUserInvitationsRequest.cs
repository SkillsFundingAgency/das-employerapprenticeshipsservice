using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUserInvitations
{
    public class GetUserInvitationsRequest : IAsyncRequest<GetUserInvitationsResponse>
    {
        public string UserId { get; set; }
    }
}