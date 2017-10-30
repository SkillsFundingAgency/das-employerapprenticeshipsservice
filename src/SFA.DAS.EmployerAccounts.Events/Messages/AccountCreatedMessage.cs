using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("add_account")]
    public class AccountCreatedMessage : PersonMessage
    {
        public AccountCreatedMessage() : base(string.Empty)
        {

        }

        public AccountCreatedMessage(long accountId, string signedByName) : base(signedByName)
        {
            AccountId = accountId;
        }
        public long AccountId { get; }
    }
}
