using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages;

[Serializable]
[MessageGroup("user_joined")]
public class UserJoinedMessage : AccountMessageBase
{
    public UserJoinedMessage()
    { }

    public UserJoinedMessage(long accountId, string creatorName, string creatorUserRef)
        : base(accountId, creatorName, creatorUserRef)
    {

    }
}