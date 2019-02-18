using System;

namespace SFA.DAS.EmployerFinance.Queries.GetExpiringAccountFunds
{
    public class GetExpiringAccountFundsResponse
    {
        public long AccountId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal? Amount { get; set; }
    }
}
