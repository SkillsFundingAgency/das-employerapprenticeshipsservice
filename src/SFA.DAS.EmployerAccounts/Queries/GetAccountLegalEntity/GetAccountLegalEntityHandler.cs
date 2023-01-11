using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntity;

public class GetAccountLegalEntityHandler : IAsyncRequestHandler<GetAccountLegalEntityRequest, GetAccountLegalEntityResponse>
{
    private readonly IEmployerAgreementRepository _employerAgreementRepository;
    private readonly IValidator<GetAccountLegalEntityRequest> _validator;

    public GetAccountLegalEntityHandler(
        IEmployerAgreementRepository employerAgreementRepository, 
        IValidator<GetAccountLegalEntityRequest> validator)
    {
        if (employerAgreementRepository == null)
            throw new ArgumentNullException(nameof(employerAgreementRepository));
        _employerAgreementRepository = employerAgreementRepository;
        _validator = validator;
    }

    public async Task<GetAccountLegalEntityResponse> Handle(GetAccountLegalEntityRequest message)
    {
        var result = _validator.Validate(message);

        if (!result.IsValid())
        {
            throw new InvalidRequestException(result.ValidationDictionary);
        }

        var accountLegalEntity = await _employerAgreementRepository.GetAccountLegalEntity(message.AccountLegalEntityId);

        return new GetAccountLegalEntityResponse
        {
            AccountLegalEntity = accountLegalEntity
        };
    }
}