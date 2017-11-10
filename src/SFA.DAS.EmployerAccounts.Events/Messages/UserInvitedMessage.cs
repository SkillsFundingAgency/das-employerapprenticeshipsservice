using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("user_invited")]
    public class UserInvitedMessage : Message
    {
        public UserInvitedMessage() : base(string.Empty, string.Empty)
        {

        }

        public UserInvitedMessage(string hashedAccountId, string signedByName) : base(signedByName, hashedAccountId)
        {

        }

    }
}
