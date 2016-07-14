using System.Collections.Generic;

namespace SFA.DAS.LevyAggregationProvider.Worker.Model
{
    public class DestinationData
    {
        public int AccountId { get; set; }

        public List<AggregationLine> Data { get; set; }
    }
}