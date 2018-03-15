using SFA.DAS.Messaging.Attributes;
using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("account_payments_processing_finished")]
    public class AccountPaymentsProcessingFinishedMessage : AccountMessageBase
    {
        public string PeriodEnd { get; }


        public AccountPaymentsProcessingFinishedMessage() : base(0, string.Empty, string.Empty)
        { }

        public AccountPaymentsProcessingFinishedMessage(long accountId, string periodEnd, string creatorName, string creatorUserRef)
            : base(accountId, creatorName, creatorUserRef)
        {
            this.PeriodEnd = periodEnd;
        }
    }
}
