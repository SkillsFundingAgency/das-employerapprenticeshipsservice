using MediatR;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitations
{
    public class GetTransferConnectionInvitationsQuery : MembershipMessage, IAsyncRequest<GetTransferConnectionInvitationsResponse>
    {
    }
}