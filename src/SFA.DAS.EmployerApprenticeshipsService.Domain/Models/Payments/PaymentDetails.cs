using System;

namespace SFA.DAS.EAS.Domain.Models.Payments;

public class PaymentDetails : Payment
{
    public string PeriodEnd { get; set; }
    public string ProviderName { get; set; }
    public string CourseName { get; set; }
    public int? CourseLevel { get; set; }
    public DateTime? CourseStartDate { get; set; }
    public string ApprenticeName { get; set; }
    public string ApprenticeNINumber { get; set; }
    public bool IsHistoricProviderName { get; set; }
}
