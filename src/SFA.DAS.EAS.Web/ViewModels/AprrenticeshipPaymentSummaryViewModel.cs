namespace SFA.DAS.EAS.Web.ViewModels
{
    public class AprrenticeshipPaymentSummaryViewModel
    {
        public string ApprenticeName { get; set; }
        public decimal LevyPaymentAmount { get; set; }
        public decimal SFACoInvestmentAmount { get; set; }
        public decimal EmployerCoInvestmentAmount { get; set; }

        public decimal TotalAmount => LevyPaymentAmount + SFACoInvestmentAmount + EmployerCoInvestmentAmount;
    }
}