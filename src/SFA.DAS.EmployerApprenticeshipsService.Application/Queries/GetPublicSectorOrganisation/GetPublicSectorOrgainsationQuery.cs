using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetPublicSectorOrganisation
{
    public class GetPublicSectorOrgainsationQuery : IAsyncRequest<GetPublicSectorOrgainsationResponse>
    {
        public string SearchTerm { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
