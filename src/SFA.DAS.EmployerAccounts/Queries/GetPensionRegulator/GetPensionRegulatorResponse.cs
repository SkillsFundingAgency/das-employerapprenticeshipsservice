using SFA.DAS.EmployerAccounts.Models.PensionRegulator;

namespace SFA.DAS.EmployerAccounts.Queries.GetPensionRegulator;

public class GetPensionRegulatorResponse
{
    public IEnumerable<Organisation> Organisations { get; set; } = new List<Organisation>();
}