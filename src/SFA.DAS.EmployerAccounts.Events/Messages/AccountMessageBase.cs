using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    public abstract class AccountMessageBase
    {
        public long AccountId { get; }
        public DateTime CreatedAt { get; }
        public string CreatedBy { get; }

        protected AccountMessageBase()
        { }

        protected AccountMessageBase(long accountId, string createdBy)
        {
            AccountId = accountId;
            CreatedAt = DateTime.Now;
            CreatedBy = createdBy;
        }
    }
}