using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("add_paye_scheme")]
    public class PayeSchemeCreatedMessage : AccountMessageBase
    {
        public string PayeScheme { get; }

        public PayeSchemeCreatedMessage()
        { }

        public PayeSchemeCreatedMessage(string payeScheme, long accountId, string createdBy) 
            : base(accountId, createdBy)
        {
            PayeScheme = payeScheme;
        }
    }
}