using System;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.LevyAggregationProvider.Worker.Model
{
    public class SourceDataItem
    {
        public int Id { get; set; }
        public string EmpRef { get; set; }
        public decimal Amount { get; set; }
        public decimal EnglishFraction { get; set; }
        public DateTime ActivityDate { get; set; }
        public LevyItemType LevyItemType { get; set; }
    }
}