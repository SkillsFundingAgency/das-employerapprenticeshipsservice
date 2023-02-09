using System.Threading;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesCountByHashedAccountId;

public class GetAccountLegalEntitiesCountByHashedAccountIdQueryHandler : IRequestHandler<GetAccountLegalEntitiesCountByHashedAccountIdRequest, GetAccountLegalEntitiesCountByHashedAccountIdResponse>
{
    private readonly IEncodingService _encodingService;
    private readonly Lazy<EmployerAccountsDbContext> _db;
    private readonly IValidator<GetAccountLegalEntitiesCountByHashedAccountIdRequest> _validator;

    public GetAccountLegalEntitiesCountByHashedAccountIdQueryHandler(
        IEncodingService encodingService,
        Lazy<EmployerAccountsDbContext> db,
        IValidator<GetAccountLegalEntitiesCountByHashedAccountIdRequest> validator)
    {
        _encodingService = encodingService;
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

        if (_encodingService.TryDecode(message.HashedAccountId, EncodingType.AccountId, out var accountId))
        {
            var accountSpecificLegalEntity = await _db.Value.AccountLegalEntities.CountAsync(ale => ale.AccountId == accountId && !ale.Deleted.HasValue, cancellationToken: cancellationToken);

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