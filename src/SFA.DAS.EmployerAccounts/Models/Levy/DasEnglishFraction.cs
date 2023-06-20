namespace SFA.DAS.EmployerAccounts.Models.Levy;

public class DasEnglishFraction
{
    public string Id { get; set; }
    public DateTime DateCalculated { get; set; }
    public decimal Amount { get; set; }
    public string EmpRef { get; set; }
}