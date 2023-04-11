using System.Threading;

namespace SFA.DAS.EmployerAccounts.Queries.GetPensionRegulator;

public class GetPensionRegulatorQueryHandler : IRequestHandler<GetPensionRegulatorRequest, GetPensionRegulatorResponse>
{
    private readonly IValidator<GetPensionRegulatorRequest> _validator;
    private readonly IPensionRegulatorService _pensionRegulatorService;

    public GetPensionRegulatorQueryHandler(IValidator<GetPensionRegulatorRequest> validator, IPensionRegulatorService pensionRegulatorService)
    {
        _validator = validator;
        _pensionRegulatorService = pensionRegulatorService;
    }

    public async Task<GetPensionRegulatorResponse> Handle(GetPensionRegulatorRequest message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);

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