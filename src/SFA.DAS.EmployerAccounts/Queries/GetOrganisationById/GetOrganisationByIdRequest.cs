using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisationById;

public class GetOrganisationByIdRequest : IRequest<GetOrganisationByIdResponse>
{
    public OrganisationType OrganisationType { get; set; }
    public string Identifier { get; set; }
}