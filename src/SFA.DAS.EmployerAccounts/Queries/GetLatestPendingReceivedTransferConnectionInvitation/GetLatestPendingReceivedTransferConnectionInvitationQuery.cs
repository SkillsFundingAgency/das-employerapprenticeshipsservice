using MediatR;
using SFA.DAS.EmployerAccounts.Messages;

namespace SFA.DAS.EmployerAccounts.Queries.GetLatestPendingReceivedTransferConnectionInvitation
{
    public class GetLatestPendingReceivedTransferConnectionInvitationQuery : AccountMessage, IAsyncRequest<GetLatestPendingReceivedTransferConnectionInvitationResponse>
    {
    }
}