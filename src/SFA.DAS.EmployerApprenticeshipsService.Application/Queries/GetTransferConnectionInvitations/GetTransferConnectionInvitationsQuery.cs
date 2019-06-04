using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitations
{
    public class GetTransferConnectionInvitationsQuery : MembershipMessage, IAsyncRequest<GetTransferConnectionInvitationsResponse>
    {
    }
}