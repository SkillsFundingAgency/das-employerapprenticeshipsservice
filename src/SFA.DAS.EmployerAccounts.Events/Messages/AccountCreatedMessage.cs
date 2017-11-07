using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("add_account")]
    public class AccountCreatedMessage : Message
    {
        public AccountCreatedMessage() : base(string.Empty, string.Empty)
        {

        }

        public AccountCreatedMessage(string hashedAccountId, string signedByName) : base(signedByName, hashedAccountId)
        {

        }

    }
}
