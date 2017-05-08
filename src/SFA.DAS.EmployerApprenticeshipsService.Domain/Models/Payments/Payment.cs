using System;
using SFA.DAS.Provider.Events.Api.Types;


namespace SFA.DAS.EAS.Domain.Models.Payments
{
    public class Payment
    {
        public string Id { get; set; }
        public long Ukprn { get; set; }
        public long Uln { get; set; }
        public string EmployerAccountId { get; set; }
        public long? ApprenticeshipId { get; set; }
        public int DeliveryPeriodMonth { get; set; }
        public int DeliveryPeriodYear { get; set; }
        public string CollectionPeriodId { get; set; }
        public int CollectionPeriodMonth { get; set; }
        public int CollectionPeriodYear { get; set; }
        public DateTime EvidenceSubmittedOn { get; set; }
        public string EmployerAccountVersion { get; set; }
        public string ApprenticeshipVersion { get; set; }
        public FundingSource FundingSource { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public long? StandardCode { get; set; }
        public int? FrameworkCode { get; set; }
        public int? ProgrammeType { get; set; }
        public int? PathwayCode { get; set; }
        public ContractType ContractType { get; set; }
    }
}
