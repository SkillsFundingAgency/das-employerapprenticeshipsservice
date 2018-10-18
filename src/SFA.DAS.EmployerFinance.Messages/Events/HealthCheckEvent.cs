using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerFinance.Messages.Events
{
    public class HealthCheckEvent : Event
    {
        public int Id { get; set; }
    }
}