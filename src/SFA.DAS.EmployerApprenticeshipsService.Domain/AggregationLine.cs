using System.Collections.Generic;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain
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