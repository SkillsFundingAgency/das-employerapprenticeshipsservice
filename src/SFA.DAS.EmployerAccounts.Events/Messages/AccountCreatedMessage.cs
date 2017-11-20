using Newtonsoft.Json.Serialization;
using SFA.DAS.Messaging.Attributes;
using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("add_account")]
    public class AccountCreatedMessage : Message
    {
        public string CreatedByName { get; }

        public AccountCreatedMessage()
        {}

        public AccountCreatedMessage(long accountId, string createdByName) : base(accountId)
        {
            CreatedByName = createdByName;
        }

    }
}
