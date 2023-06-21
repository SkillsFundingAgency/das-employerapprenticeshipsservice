using SFA.DAS.Provider.Events.Api.Types;
using System;


namespace SFA.DAS.EAS.Domain.Models.Payments;

public class Payment
{
    public Guid Id { get; set; }
    public long Ukprn { get; set; }
    public long Uln { get; set; }
    public long EmployerAccountId { get; set; }
    public long ApprenticeshipId { get; set; }
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
    public string PathwayName { get; set; }

    public override bool Equals(object obj)
    {
        var payment = obj as Payment;

        if (payment == null) return false;

        if (!Id.Equals(payment.Id))
        {
            return false;
        }

        if (Ukprn != payment.Ukprn) return false;
        if (Uln != payment.Uln) return false;
        if (EmployerAccountId != payment.EmployerAccountId) return false;
        if (ApprenticeshipId != payment.ApprenticeshipId) return false;
        if (DeliveryPeriodMonth != payment.DeliveryPeriodMonth) return false;
        if (DeliveryPeriodYear != payment.DeliveryPeriodYear) return false;
        if (CollectionPeriodId != payment.CollectionPeriodId) return false;
        if (CollectionPeriodMonth != payment.CollectionPeriodMonth) return false;
        if (CollectionPeriodYear != payment.CollectionPeriodYear) return false;
        if (EvidenceSubmittedOn != payment.EvidenceSubmittedOn) return false;
        if (EmployerAccountVersion != payment.EmployerAccountVersion) return false;
        if (ApprenticeshipVersion != payment.ApprenticeshipVersion) return false;
        if (FundingSource != payment.FundingSource) return false;
        if (TransactionType != payment.TransactionType) return false;
        if (Amount != payment.Amount) return false;
        if (StandardCode != payment.StandardCode) return false;
        if (FrameworkCode != payment.FrameworkCode) return false;
        if (ProgrammeType != payment.ProgrammeType) return false;
        if (PathwayCode != payment.PathwayCode) return false;
        if (PathwayName != payment.PathwayName) return false;

        return true;
    }

    public override int GetHashCode()
    {
        return -1;
    }
}
