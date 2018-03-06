using MediatR;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionRoles
{
    public class GetTransferConnectionRolesQuery : MembershipMessage, IAsyncRequest<GetTransferConnectionRolesResponse>
    {
    }
}