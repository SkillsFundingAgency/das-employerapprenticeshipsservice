using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Validation;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetPostcodeAddress
{
    public class GetPostcodeAddressHandler : IAsyncRequestHandler<GetPostcodeAddressRequest, GetPostcodeAddressResponse>
    {
        private readonly IAddressLookupService _addressLookupService;
        private readonly IValidator<GetPostcodeAddressRequest> _validator;

        public GetPostcodeAddressHandler(IAddressLookupService addressLookupService, IValidator<GetPostcodeAddressRequest> validator)
        {
            _addressLookupService = addressLookupService;
            _validator = validator;
        }

        public async Task<GetPostcodeAddressResponse> Handle(GetPostcodeAddressRequest request)
        {
            var validationResult = _validator.Validate(request);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var addresses = await _addressLookupService.GetAddressesByPostcode(request.Postcode);

            return new GetPostcodeAddressResponse {Addresses = addresses};
        }
    }
}
