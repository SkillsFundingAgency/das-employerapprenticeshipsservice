namespace SFA.DAS.EAS.Account.Api.Types
{
    public class TransferConnectionViewModel
    {
        public long SenderAccountId { get; set; }
        public string SenderAccountHashedId { get; set; }
        public string SenderAccountPublicHashedId { get; set; }
        public string SenderAccountName { get; set; }
    }
}