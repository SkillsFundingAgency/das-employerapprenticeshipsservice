using System;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class CoursePaymentSummaryViewModel
    {
        public string CourseName { get; set; }
        public int? CourseLevel { get; set; }
        public string PathwayName { get; set; }
        public int? PathwayCode { get; set; }
        public DateTime? CourseStartDate { get; set; }
        public decimal LevyPaymentAmount { get; set; }
        public decimal SFACoInvestmentAmount { get; set; }
        public decimal EmployerCoInvestmentAmount { get; set; }

        public decimal TotalAmount => LevyPaymentAmount + SFACoInvestmentAmount + EmployerCoInvestmentAmount;
    }
}