using System.Threading;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.EmployerAccounts.Queries.GetUnsignedEmployerAgreement;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Queries.GetNextUnsignedEmployerAgreement;

public class GetNextUnsignedEmployerAgreementQueryHandler : IRequestHandler<GetNextUnsignedEmployerAgreementRequest, GetNextUnsignedEmployerAgreementResponse>
{
    private readonly Lazy<EmployerAccountsDbContext> _db;
    private readonly IEncodingService _encodingService;
    private readonly IValidator<GetNextUnsignedEmployerAgreementRequest> _validator;

    public GetNextUnsignedEmployerAgreementQueryHandler(
        Lazy<EmployerAccountsDbContext> db,
        IEncodingService encodingService,
        IValidator<GetNextUnsignedEmployerAgreementRequest> validator
    )
    {
        _db = db;
        _encodingService = encodingService;
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

        var accountId = _encodingService.Decode(message.HashedAccountId, EncodingType.AccountId);

        var pendingAgreementId = await _db.Value.AccountLegalEntities
            .Where(x => x.AccountId == accountId && x.PendingAgreementId != null)
            .Select(x => x.PendingAgreementId)
            .FirstOrDefaultAsync(cancellationToken);

        return new GetNextUnsignedEmployerAgreementResponse
        {
            AgreementId = pendingAgreementId
        };
    }
}