using SFA.DAS.Messaging.Attributes;
using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("add_account")]
    public class AccountCreatedMessage : AccountMessageBase
    {
        public AccountCreatedMessage() : base(0, string.Empty, Guid.NewGuid())
        {}

        public AccountCreatedMessage(long accountId, string creatorName, Guid creatorUserRef) : base(accountId, creatorName, creatorUserRef)
        { 

        }
    }
}
