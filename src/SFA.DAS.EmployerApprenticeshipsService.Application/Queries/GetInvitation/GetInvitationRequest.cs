using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetInvitation
{
    public class GetInvitationRequest : IAsyncRequest<GetInvitationResponse>
    {
        public long Id { get; set; }
    }
}