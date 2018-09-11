using System.Collections.Generic;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferConnections
{
    public class GetTransferConnectionsResponse
    {
        public IEnumerable<TransferConnectionViewModel> TransferConnections { get; set; }
    }
}