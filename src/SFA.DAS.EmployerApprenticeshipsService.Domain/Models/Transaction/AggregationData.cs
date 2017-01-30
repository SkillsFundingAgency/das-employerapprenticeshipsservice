using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Models.Transaction
{
    public class AggregationData
    {
        public long AccountId { get; set; }
        public string HashedAccountId { get; set; }
        public ICollection<TransactionLine> TransactionLines { get; set; }
    }
}