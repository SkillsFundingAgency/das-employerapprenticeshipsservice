using System;

namespace SFA.DAS.EAS.Domain.Models.Transfers;

public class AccountTransfer
{
    public long SenderAccountId { get; set; }
    public string SenderAccountName { get; set; }
    public long ReceiverAccountId { get; set; }
    public string ReceiverAccountName { get; set; }
    public long CommitmentId { get; set; }
    public string CourseName { get; set; }
    public int? CourseLevel { get; set; }
    public uint ApprenticeCount { get; set; }
    public string PeriodEnd { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; }
    public Guid RequiredPaymentId { get; set; }
}
