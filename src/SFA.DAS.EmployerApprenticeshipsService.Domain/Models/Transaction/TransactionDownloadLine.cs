using System;
using System.Globalization;

namespace SFA.DAS.EAS.Domain.Models.Transaction;

public class TransactionDownloadLine
{
    public DateTime DateCreated { get; set; }

    public string TransactionType { get; set; }

    public string PayeScheme { get; set; }

    public string PeriodEnd { get; set; }

    public string PayrollYear { get; set; }

    public int PayrollMonth { get; set; }

    public decimal LevyDeclared { get; set; }

    public string LevyDeclaredFormatted => LevyDeclared.ToString("0.00", NumberFormatInfo.InvariantInfo);

    public decimal EnglishFraction { get; set; }

    public string EnglishFractionFormatted => (100 * EnglishFraction).ToString("0.000", NumberFormatInfo.InvariantInfo);

    public decimal TenPercentTopUp { get; set; }

    public string TenPercentTopUpFormatted => TenPercentTopUp.ToString("0.00", NumberFormatInfo.InvariantInfo);

    public string TrainingProvider { get; set; }

    public string CohortReference { get; set; }

    public string Uln { get; set; }

    public string Apprentice { get; set; }

    public string ApprenticeTrainingCourse { get; set; }

    public string ApprenticeTrainingCourseLevel { get; set; }

    public decimal PaidFromLevy { get; set; }

    public string PaidFromLevyFormatted => PaidFromLevy.ToString("0.00", NumberFormatInfo.InvariantInfo);

    public decimal EmployerContribution { get; set; }

    public string EmployerContributionFormatted => EmployerContribution.ToString("0.00", NumberFormatInfo.InvariantInfo);

    public decimal GovermentContribution { get; set; }

    public string GovermentContributionFormatted => GovermentContribution.ToString("0.00", NumberFormatInfo.InvariantInfo);

    public decimal Total { get; set; }

    public string TotalFormatted => Total.ToString("0.00", NumberFormatInfo.InvariantInfo);
}
