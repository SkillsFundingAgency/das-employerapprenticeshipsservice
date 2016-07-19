using System;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain
{
    public class LevyDeclarationView
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string EmpRef { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string SubmissionId { get; set; }
        public string SubmissionType { get; set; }
        public decimal Amount { get; set; }
        public decimal EnglishFraction { get; set; }
    }
}