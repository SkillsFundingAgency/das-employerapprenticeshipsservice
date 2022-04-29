using System;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses.Transfers
{
    public class GetFinancialBreakdownResponse
    {
        public decimal Commitments { get; set; }
        public decimal ApprovedPledgeApplications { get; set; }
        public decimal AcceptedPledgeApplications { get; set; }
        public decimal PledgeOriginatedCommitments { get; set; }
        public decimal TransferConnections { get; set; }
        public DateTime ProjectionStartDate { get; set; }
        public decimal AmountPledged { get; set; }
    }
}
