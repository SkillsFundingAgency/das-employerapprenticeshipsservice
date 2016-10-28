using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetInvitation
{
    public class GetInvitationRequest : IAsyncRequest<GetInvitationResponse>
    {
        public string Id { get; set; }
    }
}