using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisationsByAorn;

public class GetOrganisationsByAornQueryHandler : IAsyncRequestHandler<GetOrganisationsByAornRequest, GetOrganisationsByAornResponse>
{
    private readonly IValidator<GetOrganisationsByAornRequest> _validator;
    private readonly IPensionRegulatorService _pensionRegulatorService;

    public GetOrganisationsByAornQueryHandler(IValidator<GetOrganisationsByAornRequest> validator, IPensionRegulatorService pensionRegulatorService)
    {
        _validator = validator;
        _pensionRegulatorService = pensionRegulatorService;
    }

    public async Task<GetOrganisationsByAornResponse> Handle(GetOrganisationsByAornRequest message)
    {
        var validationResult = _validator.Validate(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        var organisations = await _pensionRegulatorService.GetOrganisationsByAorn(message.Aorn, message.PayeRef);
        return new GetOrganisationsByAornResponse { Organisations = organisations };
    }
}