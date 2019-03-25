using System;

namespace SFA.DAS.EmployerFinance.Web.ViewModels
{
    public class FinanceDashboardViewModel
    {
        public string AccountHashedId { get; set; }
        public decimal CurrentLevyFunds { get; set; }
        public decimal? ExpiringFunds { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}