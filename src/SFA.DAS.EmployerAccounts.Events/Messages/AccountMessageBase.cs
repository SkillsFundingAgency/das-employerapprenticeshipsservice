using System;
using NServiceBus;

namespace SFA.DAS.EmployerAccounts.Events.Messages;

[Serializable]
public abstract class AccountMessageBase : IEvent
{
    //We have protected setters to support json serialization due to the empty constructor
    public long AccountId { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public string CreatorName { get; protected set; }
    public string CreatorUserRef { get; protected set; }

    protected AccountMessageBase()
    { }

    protected AccountMessageBase(long accountId, string creatorName, string creatorUserRef)
    {
        AccountId = accountId;
        CreatedAt = DateTime.UtcNow;
        CreatorName = creatorName;
        CreatorUserRef = creatorUserRef;
    }
}