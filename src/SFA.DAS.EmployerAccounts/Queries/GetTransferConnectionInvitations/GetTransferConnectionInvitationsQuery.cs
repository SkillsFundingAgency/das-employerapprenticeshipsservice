using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferConnectionInvitations
{
    public class GetTransferConnectionInvitationsQuery : MembershipMessage, IAsyncRequest<GetTransferConnectionInvitationsResponse>
    {
    }
}