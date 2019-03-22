using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Models.ExpiringFunds
{
    public class ExpiringAccountFunds
    {
        public long AccountId { get; set; }
        public DateTime ProjectionGenerationDate { get; set; }
        public List<ExpiringFunds> ExpiryAmounts { get; set; }
    }
}
