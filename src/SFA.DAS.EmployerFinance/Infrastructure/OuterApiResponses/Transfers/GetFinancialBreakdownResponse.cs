using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses.Transfers
{
    public class GetFinancialBreakdownResponse
    {
        public decimal Commitments { get; set; }
        public long ApprovedPledgeApplications { get; set; }
        public long AcceptedPledgeApplications { get; set; }
        public long PledgeOriginatedCommitments { get; set; }
        public long TransferConnections { get; set; }
    }
}
