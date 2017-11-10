using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("add_account")]
    public class AccountCreatedMessage : Message
    {
        public AccountCreatedMessage()
        {

        }

        public AccountCreatedMessage(long accountId, string signedByName) : base(signedByName, accountId)
        {

        }

    }
}
