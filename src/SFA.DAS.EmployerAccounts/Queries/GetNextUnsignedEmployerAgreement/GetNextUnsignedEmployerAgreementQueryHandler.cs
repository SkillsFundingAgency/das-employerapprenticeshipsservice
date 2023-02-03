using System.Threading;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.EmployerAccounts.Queries.GetUnsignedEmployerAgreement;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Queries.GetNextUnsignedEmployerAgreement;

public class GetNextUnsignedEmployerAgreementQueryHandler : IRequestHandler<GetNextUnsignedEmployerAgreementRequest, GetNextUnsignedEmployerAgreementResponse>
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

    public async Task<GetNextUnsignedEmployerAgreementResponse> Handle(GetNextUnsignedEmployerAgreementRequest message, CancellationToken cancellationToken)
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
            .Select(x => x.PendingAgreementId).FirstOrDefaultAsync(cancellationToken);

        return new GetNextUnsignedEmployerAgreementResponse
        {
            HashedAgreementId = pendingAgreementId.HasValue ? _hashingService.HashValue(pendingAgreementId.Value) : null
        };
    }
}