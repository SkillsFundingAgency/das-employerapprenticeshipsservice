using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Domain
{
    public class AggregationData
    {
        public long AccountId { get; set; }
        public string AccountHashedId { get; set; }
        public ICollection<TransactionLine> TransactionLines { get; set; }
    }
}