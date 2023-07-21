using System.Threading;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementsByAccountId;

namespace SFA.DAS.EmployerAccounts.Queries.GetMinimumSignedAgreementVersion;

public class GetMinimumSignedAgreementVersionQueryHandler : IRequestHandler<GetMinimumSignedAgreementVersionQuery, GetMinimumSignedAgreementVersionResponse>
{
    private readonly IMediator _mediator;
    private readonly IValidator<GetMinimumSignedAgreementVersionQuery> _validator;
    
    public GetMinimumSignedAgreementVersionQueryHandler(
        IMediator mediator,
        IValidator<GetMinimumSignedAgreementVersionQuery> validator)
    {
        _mediator = mediator;
        _validator = validator;
    }

    public async Task<GetMinimumSignedAgreementVersionResponse> Handle(GetMinimumSignedAgreementVersionQuery message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        var agreements = await _mediator.Send(new GetEmployerAgreementsByAccountIdRequest { AccountId = message.AccountId }, cancellationToken).ConfigureAwait(false);
        var minAgreementVersion = agreements.EmployerAgreements.Select(ea => ea.AccountLegalEntity.SignedAgreementVersion.GetValueOrDefault(0)).Min();

        return new GetMinimumSignedAgreementVersionResponse
        {
            MinimumSignedAgreementVersion = minAgreementVersion
        };
    }
}