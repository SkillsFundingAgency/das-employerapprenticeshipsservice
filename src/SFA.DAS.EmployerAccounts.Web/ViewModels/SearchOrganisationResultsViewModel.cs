using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Models.ReferenceData;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class SearchOrganisationResultsViewModel
{
    public string SearchTerm { get; set; }
    public OrganisationType? OrganisationType { get; set; }
    public PagedResponse<OrganisationDetailsViewModel> Results { get; set; }
    public bool IsExistingAccount { get; set; }
}