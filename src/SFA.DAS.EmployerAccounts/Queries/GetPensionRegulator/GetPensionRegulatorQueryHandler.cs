using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetPensionRegulator;

public class GetPensionRegulatorQueryHandler : IAsyncRequestHandler<GetPensionRegulatorRequest, GetPensionRegulatorResponse>
{
    private readonly IValidator<GetPensionRegulatorRequest> _validator;
    private readonly IPensionRegulatorService _pensionRegulatorService;

    public GetPensionRegulatorQueryHandler(IValidator<GetPensionRegulatorRequest> validator, IPensionRegulatorService pensionRegulatorService)
    {
        _validator = validator;
        _pensionRegulatorService = pensionRegulatorService;
    }

    public async Task<GetPensionRegulatorResponse> Handle(GetPensionRegulatorRequest message)
    {
        var validationResult = _validator.Validate(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        var organisations = await _pensionRegulatorService.GetOrganisationsByPayeRef(message.PayeRef);
        return organisations == null
            ? new GetPensionRegulatorResponse()
            : new GetPensionRegulatorResponse {Organisations = organisations};
    }
}