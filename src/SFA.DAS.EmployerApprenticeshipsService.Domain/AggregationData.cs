using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain
{
    public class AggregationData
    {
        public long AccountId { get; set; }
        public string HashedId { get; set; }
        public List<AggregationLine> Data { get; set; }
    }
}