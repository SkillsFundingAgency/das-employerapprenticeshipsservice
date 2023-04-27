namespace SFA.DAS.EAS.Domain.Models.Transfers;

public class AccountTransferDetails
{
    public string CourseName { get; set; }
    public int? CourseLevel { get; set; }
    public uint ApprenticeCount { get; set; }
    public decimal PaymentTotal { get; set; }
}
