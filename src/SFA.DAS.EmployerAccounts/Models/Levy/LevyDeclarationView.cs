namespace SFA.DAS.EmployerAccounts.Models.Levy;

public class LevyDeclarationView
{
    public long Id { get; set; }
    public long AccountId { get; set; }
    public string EmpRef { get; set; }
    public DateTime SubmissionDate { get; set; }
    public long SubmissionId { get; set; }
    public decimal LevyDueYtd { get; set; }
    public decimal EnglishFraction { get; set; }
    public string PayrollYear { get; set; }
    public short? PayrollMonth { get; set; }
    public int LastSubmission { get; set; }
    public decimal TopUpPercentage { get; set; }
    public decimal TopUp { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool EndOfYearAdjustment { get; set; }
    public decimal EndOfYearAdjustmentAmount { get; set; }
    public decimal LevyAllowanceForYear { get; set; }
    public DateTime? DateCeased { get; set; }
    public DateTime? InactiveFrom { get; set; }
    public DateTime? InactiveTo { get; set; }
    public long HmrcSubmissionId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal LevyDeclaredInMonth { get; set; }

    public DateTime? PayrollDate()
    {

        if (PayrollMonth == null || string.IsNullOrEmpty(PayrollYear))
        {
            return null;
        }

        var year = 2000;
        var month = 1;

        if (PayrollMonth <= 9)
        {
            year += Convert.ToInt32(PayrollYear.Split('-')[0]);
            month = (short)(PayrollMonth + 3);
        }
        else
        {
            year += Convert.ToInt32(PayrollYear.Split('-')[1]);
            month = (short)(PayrollMonth - 9);
        }

        var dateTime = new DateTime(year, month, 1);

        return dateTime;
    }
}