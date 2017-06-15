using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetOrganisations
{
    public class GetOrganisationsRequest : IAsyncRequest<GetOrganisationsResponse>
    {
        public string SearchTerm { get; set; }
    }
}