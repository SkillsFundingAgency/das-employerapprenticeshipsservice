using SFA.DAS.EmployerAccounts.Models.PensionRegulator;

namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisationsByAorn;

public class GetOrganisationsByAornResponse
{
    public IEnumerable<Organisation> Organisations { get; set; }
}