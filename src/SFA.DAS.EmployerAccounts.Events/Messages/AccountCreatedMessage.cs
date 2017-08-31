using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [QueueName("add_account")]
    public class AccountCreatedMessage
    {
        public long AccountId { get; set; }
    }
}
