namespace SFA.DAS.EmployerFinance.Web.ViewModels.Transfers
{
    public class IndexViewModel
    {
        public bool RenderCreateTransfersPledgeButton { get; set; }
        public bool RenderApplicationListButton { get; set; }
        public bool CanViewPledgesSection { get; set; }
        public bool CanViewApplySection { get; set; }
        public int PledgesCount { get; set; }
        public int ApplicationsCount { get; set; }
        public decimal RemainingTransferAllowance { get; set; }
    }
}