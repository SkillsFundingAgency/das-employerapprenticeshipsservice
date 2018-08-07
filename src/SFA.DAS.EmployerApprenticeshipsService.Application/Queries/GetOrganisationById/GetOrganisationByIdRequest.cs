using MediatR;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EAS.Application.Queries.GetOrganisationById
{
    public class GetOrganisationByIdRequest : IAsyncRequest<GetOrganisationByIdResponse>
    {
        public OrganisationType OrganisationType { get; set; }
        public string Identifier { get; set; }
    }
}