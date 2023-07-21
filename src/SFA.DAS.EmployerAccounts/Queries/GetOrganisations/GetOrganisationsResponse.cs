using SFA.DAS.EmployerAccounts.Models.ReferenceData;

namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisations;

public class GetOrganisationsResponse
{
    public PagedResponse<OrganisationName> Organisations { get; set; }
}