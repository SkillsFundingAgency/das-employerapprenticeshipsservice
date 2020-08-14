using System;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EmployerAccounts.Queries.GetLegalEntity
{
    public class GetLegalEntityQueryHandler : IAsyncRequestHandler<GetLegalEntityQuery, GetLegalEntityResponse>
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;
        private readonly IConfigurationProvider _configurationProvider;

        public GetLegalEntityQueryHandler(Lazy<EmployerAccountsDbContext> db, IConfigurationProvider configurationProvider)
        {
            _db = db;
            _configurationProvider = configurationProvider;
        }

        public async Task<GetLegalEntityResponse> Handle(GetLegalEntityQuery message)
        {
            var legalEntity = await _db.Value.AccountLegalEntities
                .Where(l =>
                    l.LegalEntityId == message.LegalEntityId &&
                    l.Account.HashedId == message.AccountHashedId &&
                    (l.PendingAgreementId != null || l.SignedAgreementId != null) &&
                    l.Deleted == null)
                .ProjectTo<LegalEntity>(_configurationProvider, new
                {
                    accountHashedId = message.AccountHashedId
                })
                .SingleOrDefaultAsync();

            // TODO: The template version number can now be the same across agreement types so this logic may fail
            var latestAgreement = legalEntity?.Agreements
                .OrderByDescending(a => a.TemplateVersionNumber)
                .FirstOrDefault();

            await SetEmailAddressForSignatures(legalEntity);

            if (legalEntity != null && latestAgreement != null)
            {
                legalEntity.AgreementSignedByName = latestAgreement.SignedByName;
                legalEntity.AgreementSignedDate = latestAgreement.SignedDate;
                legalEntity.AgreementStatus = latestAgreement.Status;
            }

            return new GetLegalEntityResponse
            {
                LegalEntity = legalEntity
            };
        }

        private async Task SetEmailAddressForSignatures(LegalEntity legalEntity)
        {
            var userIds = legalEntity?.Agreements?.Select(a => a.SignedById).ToArray();

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
}