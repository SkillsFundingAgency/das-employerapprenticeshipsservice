using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetOrganisations
{
    public class GetOrganisationsQueryHandler : IAsyncRequestHandler<GetOrganisationsRequest, GetOrganisationsResponse>
    {
        private readonly IValidator<GetOrganisationsRequest> _validator;
        private readonly IReferenceDataService _referenceDataService;

        public GetOrganisationsQueryHandler(IValidator<GetOrganisationsRequest> validator, IReferenceDataService referenceDataService)
        {
            _validator = validator;
            _referenceDataService = referenceDataService;
        }

        public async Task<GetOrganisationsResponse> Handle(GetOrganisationsRequest message)
        {
            var valdiationResult = _validator.Validate(message);

            if (!valdiationResult.IsValid())
            {
                throw new InvalidRequestException(valdiationResult.ValidationDictionary);
            }

            var organisations = await _referenceDataService.SearchOrganisations(message.SearchTerm);

            if (organisations == null)
            {
                return new GetOrganisationsResponse();
            }

            return new GetOrganisationsResponse {Organisations = organisations.Data.ToList()};
        }
    }
}
