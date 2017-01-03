using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.ReferenceData;

namespace SFA.DAS.EAS.Application.Queries.GetPublicSectorOrganisation
{
    public class GetPublicSectorOrgainsationHandler : IAsyncRequestHandler<GetPublicSectorOrgainsationQuery, GetPublicSectorOrgainsationResponse>
    {
        private readonly IValidator<GetPublicSectorOrgainsationQuery> _requestValidator;
        private readonly IReferenceDataService _referenceDataService;

        public GetPublicSectorOrgainsationHandler(
            IValidator<GetPublicSectorOrgainsationQuery> requestValidator, 
            IReferenceDataService referenceDataService)
        {
            _requestValidator = requestValidator;
            _referenceDataService = referenceDataService;
        }

        public async Task<GetPublicSectorOrgainsationResponse> Handle(GetPublicSectorOrgainsationQuery message)
        {
            var validationResult = _requestValidator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            PagedResponse<PublicSectorOrganisation> response;

            if (message.PageNumber.HasValue && message.PageSize.HasValue)
            {
                response = await _referenceDataService.SearchPublicSectorOrganisation(
                    message.SearchTerm,
                    message.PageNumber.Value,
                    message.PageSize.Value);
            }
            else if (message.PageNumber.HasValue)
            {
                response = await _referenceDataService.SearchPublicSectorOrganisation(
                   message.SearchTerm,
                   message.PageNumber.Value);
            }
            else
            {
                response = await _referenceDataService.SearchPublicSectorOrganisation(message.SearchTerm);
            }

            return new GetPublicSectorOrgainsationResponse
            {
                Organisaions = response
            };
        }
    }
}
