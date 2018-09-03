using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferRequests
{
    public class GetTransferRequestsQuery : MembershipMessage, IAsyncRequest<GetTransferRequestsResponse>
    {
    }
}