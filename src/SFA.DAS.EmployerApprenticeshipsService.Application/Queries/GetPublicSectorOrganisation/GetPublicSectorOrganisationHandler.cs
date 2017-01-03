using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetPublicSectorOrganisation
{
    public class GetPublicSectorOrganisationHandler : IAsyncRequestHandler<GetPublicSectorOrganisationQuery, GetPublicSectorOrganisationResponse>
    {
        private readonly IValidator<GetPublicSectorOrganisationQuery> _requestValidator;
        private readonly IReferenceDataService _referenceDataService;

        public GetPublicSectorOrganisationHandler(
            IValidator<GetPublicSectorOrganisationQuery> requestValidator, 
            IReferenceDataService referenceDataService)
        {
            _requestValidator = requestValidator;
            _referenceDataService = referenceDataService;
        }

        public async Task<GetPublicSectorOrganisationResponse> Handle(GetPublicSectorOrganisationQuery message)
        {
            var validationResult = _requestValidator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }
          
            var  response = await _referenceDataService.SearchPublicSectorOrganisation(
                message.SearchTerm,
                message.PageNumber,
                message.PageSize);

            return new GetPublicSectorOrganisationResponse
            {
                Organisaions = response
            };
        }
    }
}
