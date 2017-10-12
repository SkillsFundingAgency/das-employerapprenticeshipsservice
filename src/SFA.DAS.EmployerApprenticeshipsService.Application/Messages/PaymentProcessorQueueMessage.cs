using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EAS.Application.Messages
{
    [MessageGroup("refresh_payments")]
    public class PaymentProcessorQueueMessage
    {
        public string AccountPaymentUrl { get; set; }
        public long AccountId { get; set; }
        public string PeriodEndId { get; set; }
    }
}