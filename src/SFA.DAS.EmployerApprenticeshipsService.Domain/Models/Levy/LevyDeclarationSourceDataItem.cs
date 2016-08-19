using System;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy
{
    public class LevyDeclarationSourceDataItem
    {
        public int Id { get; set; }
        public string EmpRef { get; set; }
        public decimal Amount { get; set; }
        public decimal EnglishFraction { get; set; }
        public DateTime ActivityDate { get; set; }
        public LevyItemType LevyItemType { get; set; }
    }
}