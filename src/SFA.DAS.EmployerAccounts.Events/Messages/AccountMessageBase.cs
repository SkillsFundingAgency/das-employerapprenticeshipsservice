using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    public abstract class AccountMessageBase
    {
        //We have protected setters to support json serialsation due to the empty constructor
        public long AccountId { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public string CreatorName { get; protected set; }
        public Guid CreatorExternalId { get; protected set; }

        protected AccountMessageBase()
        { }

        protected AccountMessageBase(long accountId, string creatorName, Guid creatorExternalId)
        {
            AccountId = accountId;
            CreatedAt = DateTime.UtcNow;
            CreatorName = creatorName;
            CreatorExternalId = creatorExternalId;
        }
    }
}