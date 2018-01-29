namespace SFA.DAS.EAS.Account.Api.Types
{
    public class TransferConnectionViewModel
    {
        public long TransferConnectionId { get; set; }
        public string FundingEmployerHashedAccountId { get; set; }
        public long FundingEmployerAccountId { get; set; }
        public string FundingEmployerAccountName { get; set; }
    }
}