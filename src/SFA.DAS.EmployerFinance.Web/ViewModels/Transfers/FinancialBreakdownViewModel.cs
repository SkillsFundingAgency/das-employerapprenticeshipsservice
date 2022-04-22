using System;

namespace SFA.DAS.EmployerFinance.Web.ViewModels.Transfers
{
    public class FinancialBreakdownViewModel
    {
        public decimal StartingTransferAllowance { get; set; }
        public string FinancialYearString { get; set; }
        public string HashedAccountID { get; set; }
        public decimal TransferConnections { get; set; }
        public decimal FundsIn { get; set; }
        public decimal Commitments { get; set; }
        public decimal ApprovedPledgeApplications { get; set; }
        public decimal AcceptedPledgeApplications { get; set; }
        public decimal PledgeOriginatedCommitments { get; set; }
        public DateTime ProjectionStartDate { get; set; }
        public int NumberOfMonths { get; set; }
        public decimal AmountPledged { get; set; }
        public decimal TotalAvailableTransferAllowance { get { return StartingTransferAllowance - Commitments; } }
        public decimal TotalEstimatedSpend { get { return ApprovedPledgeApplications + AcceptedPledgeApplications + TransferConnections; } }
        public decimal EstimatedRemainingAllowance { get { return TotalAvailableTransferAllowance - TotalEstimatedSpend; } }        
        public decimal TotalPledgedAndTransferConnections { get { return AmountPledged + TransferConnections; } }
        public decimal TotalAvailablePledgedFunds { get { return TotalAvailableTransferAllowance - TotalPledgedAndTransferConnections; } }
    }
}