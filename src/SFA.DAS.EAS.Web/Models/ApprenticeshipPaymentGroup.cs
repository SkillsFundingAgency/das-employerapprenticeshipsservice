﻿using SFA.DAS.EAS.Domain.Models.Payments;

namespace SFA.DAS.EAS.Web.Models;

public class ApprenticeshipPaymentGroup
{
    public string ApprenticeCourseName { get; set; }
    public int? CourseLevel { get; set; }
    public DateTime? CourseStartDate { get; set; }
    public string TrainingCode { get; set; }
    public List<PaymentTransactionLine> Payments { get; set; }
}