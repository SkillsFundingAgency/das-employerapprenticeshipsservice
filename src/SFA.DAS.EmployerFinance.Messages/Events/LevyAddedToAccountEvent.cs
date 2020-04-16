using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerFinance.Messages.Events
{
    public class LevyAddedToAccountEvent : Event
    {
        public long AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}