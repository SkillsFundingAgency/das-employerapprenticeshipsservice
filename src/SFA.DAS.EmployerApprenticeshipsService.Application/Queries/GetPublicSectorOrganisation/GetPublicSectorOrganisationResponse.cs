using SFA.DAS.EAS.Domain.Models.ReferenceData;

namespace SFA.DAS.EAS.Application.Queries.GetPublicSectorOrganisation
{
    public class GetPublicSectorOrganisationResponse
    {
        public PagedResponse<PublicSectorOrganisation> Organisaions { get; set; }
    }
}
