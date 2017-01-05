using SFA.DAS.EAS.Domain.Models.ReferenceData;

namespace SFA.DAS.EAS.Web.Models
{
    public class PublicSectorOrganisationSearchResultsViewModel
    {
        public PagedResponse<OrganisationDetailsViewModel> Results { get; set; }
    }
}