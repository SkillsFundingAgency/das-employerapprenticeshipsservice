namespace SFA.DAS.EmployerFinance.Web.ViewModels.Transfers
{
    public class FinancialBreakdownViewModel
    {
        public decimal StartingTransferAllowance { get; set; }
        public string FinancialYearString { get; set; }
        public string HashedAccountID { get; set; }
        public long TransferConnections { get; set; }
        public decimal Commitments { get; set; }
        public decimal TotalAvailableTransferAllowance { get { return StartingTransferAllowance - Commitments; } }
    }
}