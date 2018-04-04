using SFA.DAS.Messaging.Attributes;
using System;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [Serializable]
    [MessageGroup("payment_created")]
    public class PaymentCreatedMessage : AccountMessageBase
    {
        public decimal Amount { get; set; }
        public string PeriodEnd { get; set; }
        public string ProviderName { get; set; }
        public string CourseName { get; set; }
        public int? CourseLevel { get; set; }
        public DateTime? CourseStartDate { get; set; }
        public string ApprenticeName { get; set; }
        public string ApprenticeNINumber { get; set; }
        public string ApprenticeshipVersion { get; set; }
        public long ApprenticeshipId { get; set; }
        public long Ukprn { get; set; }
        public long Uln { get; set; }
        public int DeliveryPeriodMonth { get; set; }
        public int DeliveryPeriodYear { get; set; }
        public string CollectionPeriodId { get; set; }
        public int CollectionPeriodMonth { get; set; }
        public int CollectionPeriodYear { get; set; }
        public DateTime EvidenceSubmittedOn { get; set; }
        public FundingSource FundingSource { get; set; }
        public TransactionType TransactionType { get; set; }

        public PaymentCreatedMessage()
        { }

        public PaymentCreatedMessage(long accountId, string creatorName, string creatorUserRef) 
            : base(accountId, creatorName, creatorUserRef)
        {
        }
    }
}
