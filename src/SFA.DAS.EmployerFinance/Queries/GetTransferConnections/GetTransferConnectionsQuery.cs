using MediatR;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferConnections
{
    public class GetTransferConnectionsQuery : IAsyncRequest<GetTransferConnectionsResponse>
    {
        public long AccountId { get; set; }
    }
}