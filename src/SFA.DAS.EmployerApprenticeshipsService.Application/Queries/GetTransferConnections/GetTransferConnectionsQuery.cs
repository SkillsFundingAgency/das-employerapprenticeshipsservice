using MediatR;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnections
{
    public class GetTransferConnectionsQuery : AccountMessage, IAsyncRequest<GetTransferConnectionsResponse>
    {
    }
}