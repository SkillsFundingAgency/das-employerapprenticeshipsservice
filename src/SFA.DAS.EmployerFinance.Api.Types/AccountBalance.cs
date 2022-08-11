using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Api.Types
{
    public class AccountBalance
    {
        public long AccountId { get; set; }
        public decimal Balance { get; set; }
        public decimal RemainingTransferAllowance { get; set; }
        public decimal StartingTransferAllowance { get; set; }
        public int IsLevyPayer { get; set; }
        public bool? LevyOverride { get; set; }
    }
}
