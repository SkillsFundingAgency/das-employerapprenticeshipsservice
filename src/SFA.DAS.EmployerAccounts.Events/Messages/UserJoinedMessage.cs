using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("user_joined")]
    public class UserJoinedMessage : Message
    {
        public string WhoJoinedName { get; }

        public UserJoinedMessage()
        { }

        public UserJoinedMessage(long accountId, string whoJoinedName) : base(accountId)
        {
            WhoJoinedName = whoJoinedName;
        }

    }
}
