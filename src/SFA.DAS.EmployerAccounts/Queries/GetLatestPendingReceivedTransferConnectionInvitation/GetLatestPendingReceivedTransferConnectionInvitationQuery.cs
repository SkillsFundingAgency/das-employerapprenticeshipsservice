using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerAccounts.Queries.GetLatestPendingReceivedTransferConnectionInvitation
{
    public class GetLatestPendingReceivedTransferConnectionInvitationQuery : AccountMessage, IAsyncRequest<GetLatestPendingReceivedTransferConnectionInvitationResponse>
    {
    }
}