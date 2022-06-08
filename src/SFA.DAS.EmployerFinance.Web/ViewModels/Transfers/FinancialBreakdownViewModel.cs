using System;

namespace SFA.DAS.EmployerFinance.Web.ViewModels.Transfers
{
    public class FinancialBreakdownViewModel
    {
        public decimal StartingTransferAllowance { get; set; }
        public string FinancialYearString { get; set; }
        public string NextFinancialYearString { get; set; }
        public string YearAfterNextFinancialYearString { get; set; }
        public string HashedAccountID { get; set; }
        public decimal TransferConnections { get; set; }        
        public decimal Commitments { get; set; }
        public decimal ApprovedPledgeApplications { get; set; }
        public decimal AcceptedPledgeApplications { get; set; }
        public decimal PledgeOriginatedCommitments { get; set; }
        public DateTime ProjectionStartDate { get; set; }        
        public decimal AmountPledged { get; set; }
        public decimal TotalAvailableTransferAllowance { get { return StartingTransferAllowance - Commitments; } }
        public decimal CurrentYearEstimatedSpend { get; set; }
        public decimal NextYearEstimatedSpend { get; set; }
        public decimal YearAfterNextYearEstimatedSpend { get; set; }
        public decimal EstimatedRemainingAllowance { get { return StartingTransferAllowance - CurrentYearEstimatedSpend; } }
        public decimal TotalPledgedAndTransferConnections { get { return AmountPledged + TransferConnections; } }
        public decimal TotalAvailablePledgedFunds { get { return TotalAvailableTransferAllowance - TotalPledgedAndTransferConnections; } }
        public decimal AvailablePledgedFunds { get { return AmountPledged - (ApprovedPledgeApplications + AcceptedPledgeApplications); } }
    }
}