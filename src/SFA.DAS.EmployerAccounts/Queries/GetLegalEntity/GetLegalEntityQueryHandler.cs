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
                    l.LegalEntityId == message.LegalEntityId.Value &&
                    l.AccountId == message.AccountId.Value && 
                    (l.PendingAgreementId != null || l.SignedAgreementId != null))
                .ProjectTo<LegalEntityViewModel>(_configurationProvider, new
                {
                    accountId = message.AccountId.Value,
                    accountHashedId = message.AccountHashedId
                })
                .SingleOrDefaultAsync();

            // TODO: The template version number can now be the same across agreement types so this logic may fail
            var latestAgreement = legalEntity?.Agreements
                .OrderByDescending(a => a.TemplateVersionNumber)
                .FirstOrDefault();

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
    }
}