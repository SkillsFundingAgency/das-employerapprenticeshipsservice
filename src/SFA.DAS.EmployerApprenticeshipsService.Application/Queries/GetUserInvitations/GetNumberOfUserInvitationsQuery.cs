using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUserInvitations
{
    public class GetNumberOfUserInvitationsQuery : IAsyncRequest<GetNumberOfUserInvitationsResponse>
    {
        public string UserId { get; set; }
    }
}