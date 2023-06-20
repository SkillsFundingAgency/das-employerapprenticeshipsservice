using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisations;

public class GetOrganisationsRequest : IRequest<GetOrganisationsResponse>
{
    public string SearchTerm { get; set; }
    public int PageNumber { get; set; }
    public OrganisationType? OrganisationType { get; set; }
}