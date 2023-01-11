using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetCharity;

public class GetCharityQueryHandler : IAsyncRequestHandler<GetCharityQueryRequest, GetCharityQueryResponse>
{
    private readonly IReferenceDataService _referenceDataService;
    private readonly IValidator<GetCharityQueryRequest> _validator;

    public GetCharityQueryHandler(IReferenceDataService referenceDataService, IValidator<GetCharityQueryRequest> validator)
    {
        _referenceDataService = referenceDataService;
        _validator = validator;
    }

    public async Task<GetCharityQueryResponse> Handle(GetCharityQueryRequest message)
    {

        var validationResult = _validator.Validate(message);
        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        var charity = await _referenceDataService.GetCharity(message.RegistrationNumber);

        return new GetCharityQueryResponse
        {
            Charity = charity
        };
    }

}