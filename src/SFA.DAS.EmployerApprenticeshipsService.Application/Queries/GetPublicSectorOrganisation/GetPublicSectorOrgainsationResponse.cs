using SFA.DAS.EAS.Domain.Models.ReferenceData;

namespace SFA.DAS.EAS.Application.Queries.GetPublicSectorOrganisation
{
    public class GetPublicSectorOrgainsationResponse
    {
        public PagedResponse<PublicSectorOrganisation> Organisaions { get; set; }
    }
}
