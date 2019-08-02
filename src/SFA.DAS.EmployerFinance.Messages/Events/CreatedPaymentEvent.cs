using SFA.DAS.NServiceBus;

#pragma warning disable 618
namespace SFA.DAS.EmployerFinance.Messages.Events
{

    public class CreatedPaymentEvent : Event
    {
        public long AccountId { get; set; }
        public decimal Amount { get; set; }
        public string ProviderName { get; set; }
    }
}
#pragma warning restore 618