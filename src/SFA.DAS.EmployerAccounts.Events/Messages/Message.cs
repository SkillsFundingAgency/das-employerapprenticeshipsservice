using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    public abstract class Message
    {
        protected Message(string signedByName, string hashedAccountId)
        {
            SignedByName = signedByName;
            HashedAccountId = hashedAccountId;
            PostedDatedTime = DateTime.Now;
        }

        public string SignedByName { get; }

        public DateTime PostedDatedTime { get; }

        public string HashedAccountId { get; set; }
    }
}
