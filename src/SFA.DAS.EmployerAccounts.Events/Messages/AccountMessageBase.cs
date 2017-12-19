using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    public abstract class AccountMessageBase
    {
        public long AccountId { get; }
        public DateTime CreatedAt { get; }
        public string CreatorName { get; }
        public string CreatorUserRef { get; }

        protected AccountMessageBase()
        { }

        protected AccountMessageBase(long accountId, string creatorName, string creatorUserRef)
        {
            AccountId = accountId;
            CreatedAt = DateTime.Now;
            CreatorName = creatorName;
            CreatorUserRef = creatorUserRef;
        }
    }
}