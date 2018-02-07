using MediatR;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitations
{
    public class GetTransferConnectionInvitationsQuery : AuthorizedMessage, IAsyncRequest<GetTransferConnectionInvitationsResponse>
    {
    }
}