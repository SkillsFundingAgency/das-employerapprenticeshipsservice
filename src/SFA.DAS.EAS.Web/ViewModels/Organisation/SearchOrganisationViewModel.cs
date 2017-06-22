using SFA.DAS.EAS.Domain.Models.ReferenceData;

namespace SFA.DAS.EAS.Web.ViewModels.Organisation
{
    public class SearchOrganisationViewModel
    {
        public string SearchTerm { get; set; }
        public PagedResponse<Domain.Models.ReferenceData.Organisation> Results { get; set; }
    }
}