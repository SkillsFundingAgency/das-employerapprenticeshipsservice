namespace SFA.DAS.EmployerAccounts.Commands.CreateOrganisationAddress;

public class CreateOrganisationAddressRequest : IRequest<CreateOrganisationAddressResponse>
{
    public string AddressFirstLine { get; set; }
    public string AddressSecondLine { get; set; }
    public string AddressThirdLine { get; set; }
    public string TownOrCity { get; set; }
    public string County { get; set; }
    public string Postcode { get; set; }
}