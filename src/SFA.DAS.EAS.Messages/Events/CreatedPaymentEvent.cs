using NServiceBus;
using System;

namespace SFA.DAS.EAS.Messages.Events
{
    public class CreatedPaymentEvent : IEvent
    {
        public long AccountId { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Amount { get; set; }
        public string ProviderName { get; set; }
    }
}
