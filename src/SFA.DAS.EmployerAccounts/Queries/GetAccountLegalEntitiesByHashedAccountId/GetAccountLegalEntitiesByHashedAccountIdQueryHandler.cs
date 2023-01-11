using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesByHashedAccountId;

public class GetAccountLegalEntitiesByHashedAccountIdQueryHandler : IAsyncRequestHandler<GetAccountLegalEntitiesByHashedAccountIdRequest, GetAccountLegalEntitiesByHashedAccountIdResponse>
{
    private readonly IAccountLegalEntityRepository _accountLegalEntityRepository;
    private readonly IValidator<GetAccountLegalEntitiesByHashedAccountIdRequest> _validator;

    public GetAccountLegalEntitiesByHashedAccountIdQueryHandler(IAccountLegalEntityRepository accountLegalEntityRepository,
        IValidator<GetAccountLegalEntitiesByHashedAccountIdRequest> validator)
    {
        _accountLegalEntityRepository = accountLegalEntityRepository;
        _validator = validator;
    }

    public async Task<GetAccountLegalEntitiesByHashedAccountIdResponse> Handle(GetAccountLegalEntitiesByHashedAccountIdRequest message)
    {
        var result = _validator.Validate(message);

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