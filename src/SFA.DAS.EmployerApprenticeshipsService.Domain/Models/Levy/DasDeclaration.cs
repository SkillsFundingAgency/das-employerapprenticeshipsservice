using System;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy
{
    public class DasDeclaration
    {
        public string Id { get; set; }

        public decimal LevyDueYtd { get; set; }

        public DateTime Date { get; set; }
        public string SubmissionType { get; set; }
        public decimal LevyAllowanceForFullYear { get; set; }
        public string PayrollYear { get; set; }
        public short? PayrollMonth { get; set; }
    }
}