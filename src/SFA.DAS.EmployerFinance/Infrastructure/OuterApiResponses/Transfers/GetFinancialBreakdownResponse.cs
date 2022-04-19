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
        public decimal ApprovedPledgeApplications { get; set; }
        public decimal AcceptedPledgeApplications { get; set; }
        public decimal PledgeOriginatedCommitments { get; set; }
        public decimal TransferConnections { get; set; }
        public decimal FundsIn { get; set; }
        public int NumberOfMonths { get; set; }
        public DateTime ProjectionStartDate { get; set;}


    }
}
