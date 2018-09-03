using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferConnections
{
    public class GetTransferConnectionsQuery : AccountMessage, IAsyncRequest<GetTransferConnectionsResponse>
    {
    }
}