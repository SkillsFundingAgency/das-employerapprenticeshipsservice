using System;

namespace SFA.DAS.EmployerFinance.Queries.GetAccountFinanceOverview
{
    public class GetAccountFinanceOverviewResponse
    {
        public long AccountId { get; set; }
        public decimal CurrentFunds { get; set; }
        public decimal? ExpiringFundsAmount { get; set; }
        public DateTime? ExpiringFundsExpiryDate { get; set; }
        public decimal FundingExpected { get; set; }
        public decimal ProjectedSpend { get; set; }
    }
}
