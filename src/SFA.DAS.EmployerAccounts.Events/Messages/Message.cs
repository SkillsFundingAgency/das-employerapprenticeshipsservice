using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
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
