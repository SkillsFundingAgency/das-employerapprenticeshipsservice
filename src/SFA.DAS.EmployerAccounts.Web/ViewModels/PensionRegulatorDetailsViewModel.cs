using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class PensionRegulatorDetailsViewModel : NavigationViewModel
{
    public string HashedId { get; set; }
    public OrganisationType Type { get; set; }

    public string Name { get; set; }
    public string Address { get; set; }
    public long ReferenceNumber { get; set; }
    public string Status { get; set; }
}