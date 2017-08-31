using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [QueueName("add_paye_scheme")]
    public class PayeSchemeCreatedMessage
    {
        public string EmpRef { get; set; }
    }
}