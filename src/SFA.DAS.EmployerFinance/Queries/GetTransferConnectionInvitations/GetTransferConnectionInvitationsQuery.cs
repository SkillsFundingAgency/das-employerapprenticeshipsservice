using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferConnectionInvitations
{
    public class GetTransferConnectionInvitationsQuery : MembershipMessage, IAsyncRequest<GetTransferConnectionInvitationsResponse>
    {
    }
}