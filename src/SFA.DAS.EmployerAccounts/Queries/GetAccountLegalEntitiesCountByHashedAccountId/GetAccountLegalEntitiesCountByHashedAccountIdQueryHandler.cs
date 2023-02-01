using System.Data.Entity;
using System.Threading;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesCountByHashedAccountId;

public class GetAccountLegalEntitiesCountByHashedAccountIdQueryHandler : IRequestHandler<GetAccountLegalEntitiesCountByHashedAccountIdRequest, GetAccountLegalEntitiesCountByHashedAccountIdResponse>
{
    private readonly IHashingService _hashingService;
    private readonly Lazy<EmployerAccountsDbContext> _db;
    private readonly IValidator<GetAccountLegalEntitiesCountByHashedAccountIdRequest> _validator;

    public GetAccountLegalEntitiesCountByHashedAccountIdQueryHandler(
        IHashingService hashingService,
        Lazy<EmployerAccountsDbContext> db,
        IValidator<GetAccountLegalEntitiesCountByHashedAccountIdRequest> validator)
    {
        _hashingService = hashingService;
        _db = db;
        _validator = validator;
    }

    public async Task<GetAccountLegalEntitiesCountByHashedAccountIdResponse> Handle(GetAccountLegalEntitiesCountByHashedAccountIdRequest message, CancellationToken cancellationToken)
    {
        var result = _validator.Validate(message);

        if (!result.IsValid())
        {
            throw new InvalidRequestException(result.ValidationDictionary);
        }

        if (_hashingService.TryDecodeValue(message.HashedAccountId, out var accountId))
        {
            var accountSpecificLegalEntity = await _db.Value.AccountLegalEntities.CountAsync(ale => ale.AccountId == accountId && !ale.Deleted.HasValue);

            return new GetAccountLegalEntitiesCountByHashedAccountIdResponse
            {
                LegalEntitiesCount = accountSpecificLegalEntity
            };
        }

        throw new InvalidRequestException(
            new Dictionary<string, string>
            {
                {
                    nameof(message.HashedAccountId), "Hashed account ID cannot be decoded."
                }
            }
        );
    }
}