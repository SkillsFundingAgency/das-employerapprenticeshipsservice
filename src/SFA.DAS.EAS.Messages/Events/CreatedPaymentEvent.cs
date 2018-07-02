using SFA.DAS.NServiceBus;

namespace SFA.DAS.EAS.Messages.Events
{
    public class CreatedPaymentEvent : Event
    {
        public long AccountId { get; set; }
        public decimal Amount { get; set; }
        public string ProviderName { get; set; }
    }
}
