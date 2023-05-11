using System.Threading;
using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementsByAccountId;

public class GetEmployerAgreementsByAccountIdRequestHandler : IRequestHandler<GetEmployerAgreementsByAccountIdRequest, GetEmployerAgreementsByAccountIdResponse>
{
    private readonly IEmployerAgreementRepository _employerAgreementRepository;
    private readonly IValidator<GetEmployerAgreementsByAccountIdRequest> _validator;

    public GetEmployerAgreementsByAccountIdRequestHandler(
        IEmployerAgreementRepository employerAgreementRepository,
        IValidator<GetEmployerAgreementsByAccountIdRequest> validator)
    {
        _employerAgreementRepository = employerAgreementRepository;
        _validator = validator;
    }

    public async Task<GetEmployerAgreementsByAccountIdResponse> Handle(GetEmployerAgreementsByAccountIdRequest message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);
        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        var agreements = await _employerAgreementRepository.GetAccountAgreements(message.AccountId).ConfigureAwait(false);


        return new GetEmployerAgreementsByAccountIdResponse
        {
            EmployerAgreements = agreements.ToList()
        };
    }
}