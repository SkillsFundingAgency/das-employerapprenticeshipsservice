using System.Text;
using MediatR;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.CreateOrganisationAddress
{
    public class CreateOrganisationAddressHandler : IRequestHandler<CreateOrganisationAddressRequest, CreateOrganisationAddressResponse>
    {
        private readonly IValidator<CreateOrganisationAddressRequest> _validator;

        public CreateOrganisationAddressHandler(IValidator<CreateOrganisationAddressRequest> validator)
        {
            _validator = validator;
        }

        public CreateOrganisationAddressResponse Handle(CreateOrganisationAddressRequest request)
        {
            var validationResults = _validator.Validate(request);

            if (!validationResults.IsValid())
            {
                throw new InvalidRequestException(validationResults.ValidationDictionary);
            }

            var addressBuilder = new StringBuilder();
            addressBuilder.Append(request.AddressFirstLine + ", ");
            addressBuilder.Append(string.IsNullOrEmpty(request.AddressSecondLine) ? string.Empty : request.AddressSecondLine + ", ");
            addressBuilder.Append(request.TownOrCity + ", ");
            addressBuilder.Append(string.IsNullOrEmpty(request.County) ? string.Empty : request.County + ", ");
            addressBuilder.Append(request.Postcode.Trim());
            
            return new CreateOrganisationAddressResponse
            {
                Address = addressBuilder.ToString()
            };
        }
    }
}
