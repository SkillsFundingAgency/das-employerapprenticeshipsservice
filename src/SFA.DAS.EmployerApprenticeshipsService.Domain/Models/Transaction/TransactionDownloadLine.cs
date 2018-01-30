using System;

namespace SFA.DAS.EAS.Domain.Models.Transaction
{
    public class TransactionDownloadLine
    {
        public DateTime DateCreated { get; set; }

        public string TransactionType { get; set; }

        public string PayeScheme { get; set; }

        public string PeriodEnd { get; set; }

        public string PayrollYear { get; set; }

        public int PayrollMonth { get; set; }

        public decimal LevyDeclared { get; set; }

        public decimal EnglishFraction { get; set; }

        public decimal TenPercentTopUp { get; set; }

        public string TrainingProvider { get; set; }

        public string CohortReference { get; set; }

        public string Uln { get; set; }

        public string Apprentice { get; set; }

        public string ApprenticeTrainingCourse { get; set; }

        public string ApprenticeTrainingCourseLevel { get; set; }

        public decimal PaidFromLevy { get; set; }

        public decimal EmployerContribution { get; set; }

        public decimal GovermentContribution { get; set; }

        public decimal Total { get; set; }
    }
}
