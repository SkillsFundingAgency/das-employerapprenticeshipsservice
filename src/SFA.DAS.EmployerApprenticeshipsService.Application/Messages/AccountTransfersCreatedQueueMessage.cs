using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EAS.Application.Messages
{
    [MessageGroup("account_transfers_created")]
    public class AccountTransfersCreatedQueueMessage
    {
        public long SenderAccountId { get; set; }
        public string PeriodEnd { get; set; }
    }
}
