using System.Threading;
using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntity;

public class GetAccountLegalEntityHandler : IRequestHandler<GetAccountLegalEntityRequest, GetAccountLegalEntityResponse>
{
    private readonly IEmployerAgreementRepository _employerAgreementRepository;
    private readonly IValidator<GetAccountLegalEntityRequest> _validator;

    public GetAccountLegalEntityHandler(
        IEmployerAgreementRepository employerAgreementRepository, 
        IValidator<GetAccountLegalEntityRequest> validator)
    {
        _employerAgreementRepository = employerAgreementRepository ?? throw new ArgumentNullException(nameof(employerAgreementRepository));
        _validator = validator;
    }

    public async Task<GetAccountLegalEntityResponse> Handle(GetAccountLegalEntityRequest message, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(message);

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