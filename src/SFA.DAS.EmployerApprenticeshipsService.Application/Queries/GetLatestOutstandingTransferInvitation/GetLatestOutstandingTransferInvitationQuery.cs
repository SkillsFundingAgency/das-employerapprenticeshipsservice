using MediatR;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Queries.GetLatestOutstandingTransferInvitation
{
    public class GetLatestOutstandingTransferInvitationQuery : AuthorizedMessage, IAsyncRequest<GetLatestOutstandingTransferInvitationResponse>
    {
        public string ReceiverAccountHashedId { get; set; }
    }
}