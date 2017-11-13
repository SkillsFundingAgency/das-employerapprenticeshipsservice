using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("user_invited")]
    public class UserInvitedMessage : Message
    {
        public UserInvitedMessage()
        {

        }

        public UserInvitedMessage(string personInvited, long accountId, string signedByName) : base(signedByName, accountId)
        {
            PersonInvited = personInvited;
        }

        public string PersonInvited { get; }

    }
}
