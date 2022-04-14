using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses.Transfers
{
    public class GetFinancialBreakdownResponse
    {
        public long Commitments { get; set; }
        public long ApprovedPledgeApplications { get; set; }
        public long AcceptedPledgeApplications { get; set; }
        public long PledgeOriginatedCommitments { get; set; }
        public long TransferConnections { get; set; }
        public long FundsIn { get; set; }
        public int NumberOfMonths { get; set; }
        public DateTime ProjectionStartDate { get; set;}


    }
}
