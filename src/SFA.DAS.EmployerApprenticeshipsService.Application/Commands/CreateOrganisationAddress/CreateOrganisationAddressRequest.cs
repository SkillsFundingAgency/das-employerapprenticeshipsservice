using MediatR;

namespace SFA.DAS.EAS.Application.Commands.CreateOrganisationAddress
{
    public class CreateOrganisationAddressRequest : IRequest<CreateOrganisationAddressResponse>
    {
        public string AddressFirstLine { get; set; }
        public string AddressSecondLine { get; set; }
        public string TownOrCity { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
    }
}
