using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EAS.Application.Queries.GetTransferRequests
{
    public class GetTransferRequestsQuery : MembershipMessage, IAsyncRequest<GetTransferRequestsResponse>
    {
    }
}