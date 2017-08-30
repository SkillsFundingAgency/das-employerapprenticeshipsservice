using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [QueueName("delete_paye_scheme")]
    public class PayeSchemeDeletedMessage
    {
        public string EmpRef { get; set; }
    }
}