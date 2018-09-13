using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerFinance.Queries.GetLatestPendingReceivedTransferConnectionInvitation
{
    public class GetLatestPendingReceivedTransferConnectionInvitationQuery : AccountMessage, IAsyncRequest<GetLatestPendingReceivedTransferConnectionInvitationResponse>
    {
    }
}