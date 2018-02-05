using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetLatestOutstandingTransferInvitation
{
    public class GetLatestOutstandingTransferInvitationQuery : IAsyncRequest<GetLatestOutstandingTransferInvitationResponse>
    {
        public string ReceiverAccountHashedId { get; set; }
    }
}