using System;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy
{
    public class DasDeclaration
    {
        public string Id { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }
        public string SubmissionType { get; set; }
    }
}