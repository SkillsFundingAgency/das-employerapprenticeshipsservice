using SFA.DAS.NServiceBus;

#pragma warning disable 618
namespace SFA.DAS.EmployerFinance.Messages.Events
{
    public class LevyAddedToAccount : Event
    {
        public long AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
#pragma warning restore 618