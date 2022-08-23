using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses.Projections
{
    public class GetAccountProjectionSummaryResponse
    {
        public long AccountId { get; set; }
        public DateTime ProjectionGenerationDate { get; set; }
        public int NumberOfMonths { get; set; }
        public decimal FundsIn { get; set; }
        public decimal FundsOut { get; set; }
        public List<ExpiryAmount> ExpiryAmounts { get; set; }
    }

    public class ExpiryAmount
    {
        public decimal Amount { get; set; }
        public DateTime PayrollDate { get; set; }
    }
}
