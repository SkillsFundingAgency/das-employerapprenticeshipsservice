using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("user_joined")]
    public class UserJoinedMessage : Message
    {
        public string NameOfUserWhoJoined { get; }


        public UserJoinedMessage(long accountId, string nameOfUserWhoJoined) : base(accountId)
        {
            NameOfUserWhoJoined = nameOfUserWhoJoined;
        }

    }
}
