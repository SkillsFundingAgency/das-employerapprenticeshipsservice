using MediatR;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Queries.GetTransferRequests
{
    public class GetTransferRequestsQuery : MembershipMessage, IAsyncRequest<GetTransferRequestsResponse>
    {
    }
}