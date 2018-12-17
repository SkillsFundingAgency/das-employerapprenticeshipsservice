﻿using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetPostcodeAddress
{
    public class GetPostcodeAddressHandler : IAsyncRequestHandler<GetPostcodeAddressRequest, GetPostcodeAddressResponse>
    {
        private readonly IAddressLookupService _addressLookupService;
        private readonly IValidator<GetPostcodeAddressRequest> _validator;
        private readonly ILog _logger;

        public GetPostcodeAddressHandler(IAddressLookupService addressLookupService, IValidator<GetPostcodeAddressRequest> validator, ILog logger)
        {
            _addressLookupService = addressLookupService;
            _validator = validator;
            _logger = logger;
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
