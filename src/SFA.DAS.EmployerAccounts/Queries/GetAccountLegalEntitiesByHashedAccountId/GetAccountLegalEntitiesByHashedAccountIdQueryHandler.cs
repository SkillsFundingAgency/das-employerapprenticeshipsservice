using System.Threading;
using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesByHashedAccountId;

public class GetAccountLegalEntitiesByHashedAccountIdQueryHandler : IRequestHandler<GetAccountLegalEntitiesByHashedAccountIdRequest, GetAccountLegalEntitiesByHashedAccountIdResponse>
{
    private readonly IAccountLegalEntityRepository _accountLegalEntityRepository;
    private readonly IValidator<GetAccountLegalEntitiesByHashedAccountIdRequest> _validator;

    public GetAccountLegalEntitiesByHashedAccountIdQueryHandler(IAccountLegalEntityRepository accountLegalEntityRepository,
        IValidator<GetAccountLegalEntitiesByHashedAccountIdRequest> validator)
    {
        _accountLegalEntityRepository = accountLegalEntityRepository;
        _validator = validator;
    }

    public async Task<GetAccountLegalEntitiesByHashedAccountIdResponse> Handle(GetAccountLegalEntitiesByHashedAccountIdRequest message, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(message);

        if (!result.IsValid())
        {
            throw new InvalidRequestException(result.ValidationDictionary);
        }

        var accountLegalEntities = await _accountLegalEntityRepository.GetAccountLegalEntities(message.HashedAccountId);

        return new GetAccountLegalEntitiesByHashedAccountIdResponse
        {
            LegalEntities = accountLegalEntities.ToList()
        };
    }
}