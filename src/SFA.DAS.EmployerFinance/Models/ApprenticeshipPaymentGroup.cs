using System;
using System.Collections.Generic;
using SFA.DAS.EmployerFinance.Models.Payments;

namespace SFA.DAS.EmployerFinance.Models
{
    public class ApprenticeshipPaymentGroup
    {
        public string ApprenticeCourseName { get; set; }
        public int? CourseLevel { get; set; }
        public DateTime? CourseStartDate { get; set; }
        public string TrainingCode { get; set; }
        public List<PaymentTransactionLine> Payments { get; set; }
    }
}