using System.Data.Entity;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetUnsignedEmployerAgreement;

public class GetNextUnsignedEmployerAgreementQueryHandler : IAsyncRequestHandler<GetNextUnsignedEmployerAgreementRequest, GetNextUnsignedEmployerAgreementResponse>
{
    private readonly Lazy<EmployerAccountsDbContext> _db;
    private readonly IHashingService _hashingService;
    private readonly IValidator<GetNextUnsignedEmployerAgreementRequest> _validator;

    public GetNextUnsignedEmployerAgreementQueryHandler(
        Lazy<EmployerAccountsDbContext> db,
        IHashingService hashingService,
        IValidator<GetNextUnsignedEmployerAgreementRequest> validator
    )
    {
        _db = db;
        _hashingService = hashingService;
        _validator = validator;
    }

    public async Task<GetNextUnsignedEmployerAgreementResponse> Handle(GetNextUnsignedEmployerAgreementRequest message)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        if (validationResult.IsUnauthorized)
        {
            throw new UnauthorizedAccessException();
        }

        var accountId = _hashingService.DecodeValue(message.HashedAccountId);

        var pendingAgreementId = await _db.Value.AccountLegalEntities
            .Where(x => x.AccountId == accountId && x.PendingAgreementId != null)
            .Select(x => x.PendingAgreementId).FirstOrDefaultAsync();

        return new GetNextUnsignedEmployerAgreementResponse
        {
            HashedAgreementId = pendingAgreementId.HasValue ? _hashingService.HashValue(pendingAgreementId.Value) : null
        };
    }
}