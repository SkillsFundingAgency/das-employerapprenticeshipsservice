using AutoMapper;
using SFA.DAS.ReferenceData.Types.DTO;
using SFA.DAS.Validation;
using OrganisationType = SFA.DAS.Common.Domain.Types.OrganisationType;

namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisationById;

public class GetOrganisationByIdQueryHandler : IAsyncRequestHandler<GetOrganisationByIdRequest, GetOrganisationByIdResponse>
{
    private readonly IValidator<GetOrganisationByIdRequest> _validator;
    private readonly IReferenceDataService _referenceDataService;
    private readonly IPensionRegulatorService _pensionRegulatorService;
    private readonly IMapper _mapper;

    public GetOrganisationByIdQueryHandler(
        IValidator<GetOrganisationByIdRequest> validator, 
        IReferenceDataService referenceDataService,
        IPensionRegulatorService pensionRegulatorService,
        IMapper mapper)
    {
        _validator = validator;
        _referenceDataService = referenceDataService;
        _pensionRegulatorService = pensionRegulatorService;
        _mapper = mapper;
    }

    public async Task<GetOrganisationByIdResponse> Handle(GetOrganisationByIdRequest message)
    {
        var valdiationResult = _validator.Validate(message);

        if (!valdiationResult.IsValid())
        {
            throw new InvalidRequestException(valdiationResult.ValidationDictionary);
        }

        var organisation = message.OrganisationType == OrganisationType.PensionsRegulator ?
            _mapper.Map<Organisation>(await _pensionRegulatorService.GetOrganisationById(message.Identifier)) :
            await _referenceDataService.GetLatestDetails(message.OrganisationType, message.Identifier);

        return new GetOrganisationByIdResponse { Organisation = organisation };
    }
}