using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisationById
{
    public class GetOrganisationByIdQueryHandler : IAsyncRequestHandler<GetOrganisationByIdRequest, GetOrganisationByIdResponse>
    {
        private readonly IValidator<GetOrganisationByIdRequest> _validator;
        private readonly IReferenceDataService _referenceDataService;

        public GetOrganisationByIdQueryHandler(IValidator<GetOrganisationByIdRequest> validator, IReferenceDataService referenceDataService)
        {
            _validator = validator;
            _referenceDataService = referenceDataService;
        }

        public async Task<GetOrganisationByIdResponse> Handle(GetOrganisationByIdRequest message)
        {
            var valdiationResult = _validator.Validate(message);

            if (!valdiationResult.IsValid())
            {
                throw new InvalidRequestException(valdiationResult.ValidationDictionary);
            }

            var organisation =
                await _referenceDataService.GetLatestDetails(message.OrganisationType, message.Identifier);

            return new GetOrganisationByIdResponse { Organisation = organisation };
        }
    }
}
