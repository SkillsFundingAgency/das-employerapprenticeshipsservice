using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferConnections
{
    public class GetTransferConnectionsResponse
    {
        public IEnumerable<TransferConnection> TransferConnections { get; set; }
    }
}