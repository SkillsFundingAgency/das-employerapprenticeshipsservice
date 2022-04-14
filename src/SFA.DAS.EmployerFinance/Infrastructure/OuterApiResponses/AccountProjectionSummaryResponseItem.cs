using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses
{
    public class AccountProjectionSummaryResponseItem
    {
        public long AccountId { get; set; }
        public DateTime ProjectionGenerationDate { get; set; }
        public int NumberOfMonths { get; set; }
        public decimal FundsIn { get; set; }
        public decimal FundsOut { get; set; }
        public List<ExpiringFundsReponseItem> ExpiryAmounts { get; set; }
    }

    public class ExpiringFundsReponseItem
    {
        public decimal Amount { get; set; }
        public DateTime PayrollDate { get; set; }
    }
}
