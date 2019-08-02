using SFA.DAS.NServiceBus;

#pragma warning disable 618
namespace SFA.DAS.EmployerFinance.Messages.Events
{
    public class HealthCheckEvent : Event
    {
        public int Id { get; set; }
    }
}
#pragma warning restore 618