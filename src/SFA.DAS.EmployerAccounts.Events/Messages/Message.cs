using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    public abstract class Message
    {
        protected Message(string signedByName=null, long accountId=0)
        {
            SignedByName = signedByName;
            AccountId = accountId;
            PostedDatedTime = DateTime.Now;
        }

        public string SignedByName { get; }

        public DateTime PostedDatedTime { get; }

        public long AccountId { get; set; }
    }
}
