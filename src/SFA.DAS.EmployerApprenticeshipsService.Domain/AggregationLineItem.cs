using System;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain
{
    public class AggregationLineItem
    {
        public string Id { get; set; }
        public string EmpRef { get; set; }
        public decimal Amount { get; set; }
        public DateTime ActivityDate { get; set; }
        public LevyItemType LevyItemType { get; set; }
    }
}