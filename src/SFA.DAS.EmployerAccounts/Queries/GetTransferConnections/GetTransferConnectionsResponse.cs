using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Models.TransferConnections;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferConnections
{
    public class GetTransferConnectionsResponse
    {
        public IEnumerable<TransferConnectionViewModel> TransferConnections { get; set; }
    }
}