using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("user_invited")]
    public class UserInvitedMessage : Message
    {
        public UserInvitedMessage()
        {

        }

        public UserInvitedMessage(long accountId, string signedByName) : base(signedByName, accountId)
        {

        }

    }
}
