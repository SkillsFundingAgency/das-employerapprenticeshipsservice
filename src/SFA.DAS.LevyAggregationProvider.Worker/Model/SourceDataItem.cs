using System;

namespace SFA.DAS.LevyAggregationProvider.Worker.Model
{
    public class SourceDataItem
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime ActivityDate { get; set; }
        public LevyDataSource LevyDataSource { get; set; }
    }
}