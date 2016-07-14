using System;
using System.Collections.Generic;

namespace SFA.DAS.LevyAggregationProvider.Worker.Model
{
    public class AggregationLine
    {
        public LevyItemType LevyItemType { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public List<AggregationLineItem> Items { get; set; }
    }
}