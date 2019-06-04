using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnections
{
    public class GetTransferConnectionsQuery : AccountMessage, IAsyncRequest<GetTransferConnectionsResponse>
    {
    }
}