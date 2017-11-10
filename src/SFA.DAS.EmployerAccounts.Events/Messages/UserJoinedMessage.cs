using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("user_joined")]
    public class UserJoinedMessage : Message
    {
        public UserJoinedMessage()
        {

        }

        public UserJoinedMessage(long accountId, string signedByName) : base(signedByName, accountId)
        {

        }

    }
}
