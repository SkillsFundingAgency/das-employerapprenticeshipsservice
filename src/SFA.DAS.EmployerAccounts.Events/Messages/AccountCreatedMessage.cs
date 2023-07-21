using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages;

[Serializable]
[MessageGroup("add_account")]
public class AccountCreatedMessage : AccountMessageBase 
{
    public AccountCreatedMessage() : base(0, string.Empty, string.Empty)
    {
    }

    public AccountCreatedMessage(long accountId, string creatorName, string creatorUserRef) : base(accountId,
        creatorName, creatorUserRef)
    {
    }
}