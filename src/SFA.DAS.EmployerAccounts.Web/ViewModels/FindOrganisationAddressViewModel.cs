namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class FindOrganisationAddressViewModel : OrganisationViewModelBase
{
    public string Postcode { get; set; }

    public string PostcodeError => GetErrorMessage(nameof(Postcode));
}