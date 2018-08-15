using System;

namespace SFA.DAS.EmployerFinance.Messages.Events
{
    [Serializable]
    public abstract class PaymentMessageBase
    {
        //We have protected setters to support json serialsation due to the empty constructor
        public long AccountId { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public string CreatorName { get; protected set; }
        public string CreatorUserRef { get; protected set; }

        protected PaymentMessageBase()
        { }

        protected PaymentMessageBase(long accountId, string creatorName, string creatorUserRef)
        {
            AccountId = accountId;
            CreatedAt = DateTime.UtcNow;
            CreatorName = creatorName;
            CreatorUserRef = creatorUserRef;
        }
    }
}