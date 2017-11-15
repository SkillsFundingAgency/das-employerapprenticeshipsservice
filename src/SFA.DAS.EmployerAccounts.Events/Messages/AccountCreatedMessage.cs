using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("add_account")]
    public class AccountCreatedMessage : Message
    {
        public string CreatedByName { get; }

        public AccountCreatedMessage(long accountId, string createdByName) : base(accountId)
        {
            CreatedByName = createdByName;
        }

    }
}
