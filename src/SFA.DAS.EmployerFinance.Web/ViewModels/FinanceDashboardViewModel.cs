using System;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerFinance.Web.ViewModels
{
    public class FinanceDashboardViewModel
    {
        public string AccountHashedId { get; set; }
        public decimal CurrentLevyFunds { get; set; }
        public decimal? ExpiringFunds { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal TotalSpendForLastYear { get; set; }
        public decimal AvailableFunds { get; set; }
        public decimal FundingExpected { get; set; }
        public decimal ProjectedSpend { get; set; }
        public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
    }
}