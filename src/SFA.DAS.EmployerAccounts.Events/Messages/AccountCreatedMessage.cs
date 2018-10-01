using SFA.DAS.Messaging.Attributes;
using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("add_account")]
    public class AccountCreatedMessage : AccountMessageBase
    {
        public string Name { get; protected set; }
        public string PublicHashedId { get; protected set; }

        public AccountCreatedMessage() : base(0, string.Empty, string.Empty)
        {}

        public AccountCreatedMessage(long accountId, string publicHashedId, string name, string creatorName, string creatorUserRef) : base(accountId, creatorName, creatorUserRef)
        {
            Name = name;
            PublicHashedId = publicHashedId;
        }
    }
}
