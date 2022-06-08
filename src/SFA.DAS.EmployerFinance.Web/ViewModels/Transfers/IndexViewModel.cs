namespace SFA.DAS.EmployerFinance.Web.ViewModels.Transfers
{
    public class IndexViewModel
    {
        public bool RenderCreateTransfersPledgeButton { get; set; }        
        public bool IsLevyEmployer { get; set; }
        public int PledgesCount { get; set; }
        public int ApplicationsCount { get; set; }
        public decimal StartingTransferAllowance { get; set; }
        public string FinancialYearString { get; set; }
        public string HashedAccountID { get; set; }
    }
}