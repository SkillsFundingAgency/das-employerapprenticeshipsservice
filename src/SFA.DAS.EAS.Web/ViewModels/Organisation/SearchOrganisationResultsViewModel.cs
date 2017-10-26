using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Domain.Models.Organisation;
using SFA.DAS.EAS.Domain.Models.ReferenceData;

namespace SFA.DAS.EAS.Web.ViewModels.Organisation
{
    public class SearchOrganisationResultsViewModel
    {
        public string SearchTerm { get; set; }
        public OrganisationType? OrganisationType { get; set; }
        public PagedResponse<OrganisationDetailsViewModel> Results { get; set; }
        public bool IsExistingAccount { get; set; }
    }
}