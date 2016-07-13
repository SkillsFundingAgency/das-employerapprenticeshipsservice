using System.Collections.Generic;

namespace SFA.DAS.LevyAggregationProvider.Worker.Model
{
    public class SourceData
    {
        public int AccountId { get; set; }  

        public List<SourceDataItem> Data { get; set; }
    }
}