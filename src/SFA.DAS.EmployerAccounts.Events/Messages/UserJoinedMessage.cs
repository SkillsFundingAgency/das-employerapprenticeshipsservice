using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("user_joined")]
    public class UserJoinedMessage : Message
    {
        public UserJoinedMessage() : base(string.Empty, string.Empty)
        {

        }

        public UserJoinedMessage(string hashedAccountId, string signedByName) : base(signedByName, hashedAccountId)
        {

        }

    }
}
