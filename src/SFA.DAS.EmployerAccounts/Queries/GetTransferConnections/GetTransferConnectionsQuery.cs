using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferConnections
{
    public class GetTransferConnectionsQuery : AccountMessage, IAsyncRequest<GetTransferConnectionsResponse>
    {
    }
}