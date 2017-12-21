using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("add_paye_scheme")]
    public class PayeSchemeAddedMessage : AccountMessageBase
    {
        public string PayeScheme { get; }

        public PayeSchemeAddedMessage()
        { }

        public PayeSchemeAddedMessage(string payeScheme, long accountId, string creatorName, string creatorUserRef) 
            : base(accountId, creatorName, creatorUserRef)
        {
            PayeScheme = payeScheme;
        }
    }
}