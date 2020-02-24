using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerAccounts.Messages.Events
{
    public class UpsertedUserEvent : Event
    {
        public string UserRef { get; set; }

        public string CorrelationId { get; set; }
    }
}
