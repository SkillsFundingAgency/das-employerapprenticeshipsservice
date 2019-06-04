using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EAS.Application.Queries.GetLatestPendingReceivedTransferConnectionInvitation
{
    public class GetLatestPendingReceivedTransferConnectionInvitationQuery : AccountMessage, IAsyncRequest<GetLatestPendingReceivedTransferConnectionInvitationResponse>
    {
    }
}