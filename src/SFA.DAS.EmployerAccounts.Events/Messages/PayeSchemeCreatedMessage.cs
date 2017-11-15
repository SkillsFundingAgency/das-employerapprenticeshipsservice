using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("add_paye_scheme")]
    public class PayeSchemeCreatedMessage : Message
    {
        public string EmpRef { get; }
        public string CreatedByName { get; }

        public PayeSchemeCreatedMessage()
        { }

        public PayeSchemeCreatedMessage(string payeSchemeRef, long accountId, string createdByName) : base(accountId)
        {
            EmpRef = payeSchemeRef;
            CreatedByName = createdByName;
        }
    }
}