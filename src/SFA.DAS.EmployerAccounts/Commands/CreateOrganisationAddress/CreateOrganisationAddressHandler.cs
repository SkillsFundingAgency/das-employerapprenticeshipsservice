using System.Text;
using System.Threading;

namespace SFA.DAS.EmployerAccounts.Commands.CreateOrganisationAddress;

public class CreateOrganisationAddressHandler : IRequestHandler<CreateOrganisationAddressRequest, CreateOrganisationAddressResponse>
{
    private readonly IValidator<CreateOrganisationAddressRequest> _validator;

    public CreateOrganisationAddressHandler(IValidator<CreateOrganisationAddressRequest> validator)
    {
        _validator = validator;
    }

    public Task<CreateOrganisationAddressResponse> Handle(CreateOrganisationAddressRequest request, CancellationToken cancellationToken)
    {
        var validationResults = _validator.Validate(request);

        if (!validationResults.IsValid())
        {
            throw new InvalidRequestException(validationResults.ValidationDictionary);
        }

        var addressBuilder = new StringBuilder();
        addressBuilder.Append(request.AddressFirstLine + ", ");
        addressBuilder.Append(string.IsNullOrEmpty(request.AddressSecondLine) ? string.Empty : request.AddressSecondLine + ", ");
        addressBuilder.Append(string.IsNullOrEmpty(request.AddressThirdLine) ? string.Empty : request.AddressThirdLine + ", ");
        addressBuilder.Append(request.TownOrCity + ", ");
        addressBuilder.Append(string.IsNullOrEmpty(request.County) ? string.Empty : request.County + ", ");
        addressBuilder.Append(request.Postcode.Trim());
            
        return Task.FromResult(new CreateOrganisationAddressResponse
        {
            Address = addressBuilder.ToString()
        });
    }
}