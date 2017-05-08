using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class CoursePaymentDetailsViewModel
    {
        public string CourseName { get; set; }
        public string ProviderName { get; set; }
        public DateTime PaymentDate { get; set; }

        public ICollection<AprrenticeshipPaymentSummaryViewModel> ApprenticePayments { get; set; }
    }
}