using SFA.DAS.EmployerAccounts.Models.Levy;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerEnglishFractionHistory;

public class GetEmployerEnglishFractionHistoryResponse
{
    public IEnumerable<DasEnglishFraction> Fractions { get; set; }
    public string EmpRef { get; set; }
    public DateTime EmpRefAddedDate { get; set; }
}