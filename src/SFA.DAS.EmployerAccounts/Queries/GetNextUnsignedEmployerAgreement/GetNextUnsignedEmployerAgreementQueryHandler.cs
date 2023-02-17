﻿using System.Threading;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Queries.GetUnsignedEmployerAgreement;

namespace SFA.DAS.EmployerAccounts.Queries.GetNextUnsignedEmployerAgreement;

public class GetNextUnsignedEmployerAgreementQueryHandler : IRequestHandler<GetNextUnsignedEmployerAgreementRequest, GetNextUnsignedEmployerAgreementResponse>
{
    private readonly IEmployerAccountsDbContext _db;
    private readonly IValidator<GetNextUnsignedEmployerAgreementRequest> _validator;

    public GetNextUnsignedEmployerAgreementQueryHandler(
        IEmployerAccountsDbContext db,
        IValidator<GetNextUnsignedEmployerAgreementRequest> validator
    )
    {
        _db = db;
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

        var pendingAgreementId = await _db.AccountLegalEntities
            .Where(x => x.AccountId == message.AccountId && x.PendingAgreementId != null)
            .Select(x => x.PendingAgreementId)
            .FirstOrDefaultAsync(cancellationToken);

        return new GetNextUnsignedEmployerAgreementResponse
        {
            AgreementId = pendingAgreementId
        };
    }
}