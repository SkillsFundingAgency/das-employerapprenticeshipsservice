using System;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy
{
    public class LevyDeclarationSourceDataItem
    {
        public long Id { get; set; }
        public string EmpRef { get; set; }
        public decimal Amount { get; set; }
        public decimal EnglishFraction { get; set; }
        public DateTime SubmissionDate { get; set; }
        public LevyItemType LevyItemType { get; set; }
        public string PayrollDate { get; set; }
    }
}