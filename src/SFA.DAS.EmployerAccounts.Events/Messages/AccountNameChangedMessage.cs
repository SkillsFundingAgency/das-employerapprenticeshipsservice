using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages;

[Serializable]
[MessageGroup("account_name_changed")]
public class AccountNameChangedMessage : AccountMessageBase
{
    public string PreviousName { get; protected set; }
    public string CurrentName { get; protected set; }

    public AccountNameChangedMessage() : base(0, string.Empty, string.Empty)
    {}

    public AccountNameChangedMessage(string previousName, string currentName, long accountId, string creatorName, string creatorUserRef) : base(accountId, creatorName, creatorUserRef)
    {
        PreviousName = previousName;
        CurrentName = currentName;
    }
}