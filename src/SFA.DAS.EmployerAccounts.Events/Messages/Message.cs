using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    public class Message
    {
        public Message(string signedByName, string hashedAccountId)
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
