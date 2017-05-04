using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class ProviderPaymentsSummaryViewModel
    {
        public string ProviderName { get; set; }
        public DateTime PaymentDate { get; set; }

        public decimal LevyPaymentsTotal { get; set; }
        public decimal SFACoInvestmentsTotal { get; set; }
        public decimal EmployerCoInvestmentsTotal { get; set; }
        public decimal PaymentsTotal { get; set; }

        public ICollection<CoursePaymentSummaryViewModel> CoursePayments { get; set; }  
    }
}