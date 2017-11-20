using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    public abstract class AccountMessageBase
    {
        public long AccountId { get; }
        public DateTime Created { get; }
        public string CreatedBy { get; }

        protected AccountMessageBase()
        { }

        protected AccountMessageBase(long accountId, string createdBy)
        {
            AccountId = accountId;
            Created = DateTime.Now;
            CreatedBy = createdBy;
        }
    }
}
