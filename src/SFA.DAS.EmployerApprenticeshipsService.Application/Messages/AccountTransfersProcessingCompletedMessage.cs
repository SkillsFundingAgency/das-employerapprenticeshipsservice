using SFA.DAS.Messaging.Attributes;
using System;

namespace SFA.DAS.EAS.Application.Messages
{
    [Serializable]
    [MessageGroup("transfers_processing_completed")]
    public class AccountTransfersProcessingCompletedMessage
    {
        public long AccountId { get; set; }
        public string PeriodEnd { get; set; }
    }
}
