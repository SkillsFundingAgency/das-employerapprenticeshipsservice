using System.Threading;
using Microsoft.EntityFrameworkCore;
using AccountLegalEntity = SFA.DAS.EmployerAccounts.Models.Account.AccountLegalEntity;

namespace SFA.DAS.EmployerAccounts.Queries.GetLegalEntity;

public class GetLegalEntityQueryHandler : IRequestHandler<GetLegalEntityQuery, GetLegalEntityResponse>
{
    private readonly Lazy<EmployerAccountsDbContext> _db;

    public GetLegalEntityQueryHandler(Lazy<EmployerAccountsDbContext> db)
    {
        _db = db;
    }

    public async Task<GetLegalEntityResponse> Handle(GetLegalEntityQuery message, CancellationToken cancellationToken)
    {
        var legalEntity = await  _db.Value.AccountLegalEntities.SingleOrDefaultAsync(l =>
            l.LegalEntityId == message.LegalEntityId &&
            l.Account.HashedId == message.AccountHashedId &&
            (l.PendingAgreementId != null || l.SignedAgreementId != null) &&
            l.Deleted == null, cancellationToken);

        // TODO: The template version number can now be the same across agreement types so this logic may fail
        var latestAgreement = (legalEntity?.Agreements).MaxBy(a => a.Template.VersionNumber);

        await SetEmailAddressForSignatures(legalEntity);
            
        return new GetLegalEntityResponse
        {
            LegalEntity = legalEntity,
            LatestAgreement = latestAgreement
        };
    }

    private async Task SetEmailAddressForSignatures(AccountLegalEntity legalEntity)
    {
        var userIds = legalEntity?.Agreements?.Where(a => a.SignedById != null).Select(a => a.SignedById).ToArray();
        
        if (userIds != null && userIds.Any())
        {
            var users = await _db.Value.Users.Where(u => userIds.Contains(u.Id)).Select(u => new { u.Id, u.Email }).ToListAsync();
            legalEntity.Agreements.Where(a => a.SignedById != null).ToList().ForEach(a =>
            {
                a.SignedByEmail = users.FirstOrDefault(u => u.Id == a.SignedById)?.Email;
            });
        }
    }
}