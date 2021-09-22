namespace SFA.DAS.EmployerFinance.Web.ViewModels.Transfers
{
    public class IndexViewModel
    {
        public bool IsTransferReceiver { get; set; }
        public bool IsTransferSender { get; set; }
        public bool RenderCreateTransfersPledgeButton { get; set; }
        public bool RenderApplicationListButton { get; set; }
        public int PledgesCount { get; set; }
        public bool CanViewPledgesSection { get; set; }
        public int ApplicationsCount { get; set; }
    }
}