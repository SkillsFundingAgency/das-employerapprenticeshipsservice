using System.Threading;

namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisations;

public class GetOrganisationsQueryHandler : IRequestHandler<GetOrganisationsRequest, GetOrganisationsResponse>
{
    private readonly IValidator<GetOrganisationsRequest> _validator;
    private readonly IReferenceDataService _referenceDataService;

    public GetOrganisationsQueryHandler(IValidator<GetOrganisationsRequest> validator, IReferenceDataService referenceDataService)
    {
        _validator = validator;
        _referenceDataService = referenceDataService;
    }

    public async Task<GetOrganisationsResponse> Handle(GetOrganisationsRequest message, CancellationToken cancellationToken)
    {
        var valdiationResult = await _validator.ValidateAsync(message);

        if (!valdiationResult.IsValid())
        {
            throw new InvalidRequestException(valdiationResult.ValidationDictionary);
        }

        var organisations = await _referenceDataService.SearchOrganisations(message.SearchTerm, message.PageNumber, organisationType: message.OrganisationType);

        if (organisations == null)
        {
            return new GetOrganisationsResponse();
        }

        return new GetOrganisationsResponse { Organisations = organisations };
    }
}