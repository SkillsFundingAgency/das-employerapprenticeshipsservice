using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferConnections
{
    public class GetTransferConnectionsQuery : IAsyncRequest<GetTransferConnectionsResponse>
    {
        public long? AccountId { get; set; }
        public string HashedAccountId { get; set; }
    }
}