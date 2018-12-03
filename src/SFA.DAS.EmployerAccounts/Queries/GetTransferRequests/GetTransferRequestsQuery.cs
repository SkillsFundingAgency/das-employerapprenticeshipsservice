using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferRequests
{
    public class GetTransferRequestsQuery : MembershipMessage, IAsyncRequest<GetTransferRequestsResponse>
    {
    }
}