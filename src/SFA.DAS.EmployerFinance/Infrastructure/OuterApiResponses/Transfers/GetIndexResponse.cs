namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses.Transfers
{
    public class GetIndexResponse
    {
        public int PledgesCount { get; set; }
        public int ApplicationsCount { get; set; }
        public bool IsTransferReceiver { get; set; }
        public bool IsTransferSender { get; set; }
        public int ActivePledgesTotalAmount { get; set; }
    }
}