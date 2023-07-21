namespace SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Responses.Finance;

public class GetEnglishFractionResponse
{
    public List<DasEnglishFraction> Fractions { get; set; }
}

public class DasEnglishFraction
{
    public string Id { get; set; }
    public DateTime DateCalculated { get; set; }
    public decimal Amount { get; set; }
    public string EmpRef { get; set; }
}