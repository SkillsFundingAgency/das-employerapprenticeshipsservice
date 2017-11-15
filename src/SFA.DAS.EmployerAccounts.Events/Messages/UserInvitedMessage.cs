using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("user_invited")]
    public class UserInvitedMessage : Message
    {
        public string PersonInvited { get; }
        public string InvitedByName { get; }

        public UserInvitedMessage(string personInvited, long accountId, string invitedByName) : base(accountId)
        {
            PersonInvited = personInvited;
            InvitedByName = invitedByName;
        }
    }
}
