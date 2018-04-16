using SFA.DAS.Messaging.Attributes;
using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("account_payments_processing_completed")]
    public class AccountPaymentsProcessingCompletedMessage : AccountMessageBase
    {
        public string PeriodEnd { get; protected set; }


        public AccountPaymentsProcessingCompletedMessage() : base(0, string.Empty, string.Empty)
        { }

        public AccountPaymentsProcessingCompletedMessage(long accountId, string periodEnd, string creatorName, string creatorUserRef)
            : base(accountId, creatorName, creatorUserRef)
        {
            this.PeriodEnd = periodEnd;
        }
    }
}
