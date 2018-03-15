using SFA.DAS.Messaging.Attributes;
using System;

namespace SFA.DAS.EAS.Application.Messages
{
    [Serializable]
    [MessageGroup("account_transfers_created")]
    public class AccountTransfersCreatedQueueMessage
    {
        public long SenderAccountId { get; set; }
        public string PeriodEnd { get; set; }
    }
}
