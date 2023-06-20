using SFA.DAS.EmployerAccounts.Models.Levy;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class PayeSchemeDetailViewModel
{
    public IEnumerable<DasEnglishFraction> Fractions { get; set; }
    public string EmpRef { get; set; }
    public string PayeSchemeName { get; set; }
    public DateTime EmpRefAdded { get; set; }
}