using MediatR;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Queries.GetLatestPendingReceivedTransferConnectionInvitation
{
    public class GetLatestPendingReceivedTransferConnectionInvitationQuery : AccountMessage, IAsyncRequest<GetLatestPendingReceivedTransferConnectionInvitationResponse>
    {
    }
}