namespace SFA.DAS.EmployerFinance.Web.ViewModels.Transfers
{
    public class IndexViewModel
    {
        public bool IsTransferReceiver { get; set; }
        public bool RenderCreateTransfersPledgeButton { get; set; }
        public int PledgesCount { get; set; }
        public int ApplicationsCount { get; set; }
    }
}