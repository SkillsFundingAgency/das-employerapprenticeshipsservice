using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetInvitation
{
    public class GetInvitationRequest : IAsyncRequest<GetInvitationResponse>
    {
        public string Id { get; set; }
    }
}