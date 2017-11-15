using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    public abstract class Message
    {

        public DateTime PostedDatedTime { get; }
        public long AccountId { get; set; }

        protected Message(long accountId=0)
        {
            AccountId = accountId;
            PostedDatedTime = DateTime.Now;
        }
    }
}
