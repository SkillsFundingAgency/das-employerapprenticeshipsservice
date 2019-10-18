using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferConnections
{
    public class GetTransferConnectionsQuery : IAsyncRequest<GetTransferConnectionsResponse>
    {
        public string HashedAccountId { get; set; }
    }
}