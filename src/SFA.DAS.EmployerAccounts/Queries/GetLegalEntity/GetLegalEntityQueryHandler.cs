using MediatR;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Data;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AccountLegalEntity = SFA.DAS.EmployerAccounts.Models.Account.AccountLegalEntity;
using LegalEntity = SFA.DAS.EmployerAccounts.Api.Types.LegalEntity;

namespace SFA.DAS.EmployerAccounts.Queries.GetLegalEntity
{
    public class GetLegalEntityQueryHandler : IAsyncRequestHandler<GetLegalEntityQuery, GetLegalEntityResponse>
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public GetLegalEntityQueryHandler(Lazy<EmployerAccountsDbContext> db)
        {
            _db = db;
        }

        public async Task<GetLegalEntityResponse> Handle(GetLegalEntityQuery message)
        {
            var legalEntity = _db.Value.AccountLegalEntities.Where(l =>
                    l.LegalEntityId == message.LegalEntityId &&
                    l.Account.HashedId == message.AccountHashedId &&
                    (l.PendingAgreementId != null || l.SignedAgreementId != null) &&
                    l.Deleted == null)
                    .Select(MapToLegalEntity(message)).SingleOrDefault();

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

        private Expression<Func<AccountLegalEntity, LegalEntity>> MapToLegalEntity(GetLegalEntityQuery message)
        {
            return accountLegalEntity => new LegalEntity
            {
                Agreements = accountLegalEntity.Agreements.Where(x =>
                    message.IncludeAllAgreements || (x.StatusId == Models.EmployerAgreement.EmployerAgreementStatus.Pending ||
                                             x.StatusId == Models.EmployerAgreement.EmployerAgreementStatus.Signed)).Select(
                    a => new Agreement
                    {
                        Id = a.Id,
                        AgreementType = a.Template.AgreementType,
                        TemplateVersionNumber = a.Template.VersionNumber,
                        Status = (EmployerAgreementStatus)(int)a.StatusId,
                        SignedById = a.SignedById,
                        SignedByName = a.SignedByName,
                        SignedDate = a.SignedDate

                    }).ToList(),
                DasAccountId = message.AccountHashedId,
                AccountLegalEntityId = accountLegalEntity.Id,
                AccountLegalEntityPublicHashedId = accountLegalEntity.PublicHashedId,
                LegalEntityId = accountLegalEntity.LegalEntityId,
                DateOfInception = accountLegalEntity.LegalEntity.DateOfIncorporation,
                Code = accountLegalEntity.LegalEntity.Code,
                Sector = accountLegalEntity.LegalEntity.Sector,
                Status = accountLegalEntity.LegalEntity.Status,
                PublicSectorDataSource = accountLegalEntity.LegalEntity.PublicSectorDataSource == 1 ? "ONS" : accountLegalEntity.LegalEntity.PublicSectorDataSource == 2 ? "NHS" : accountLegalEntity.LegalEntity.PublicSectorDataSource == 3 ? "Police" : "",
                Source = accountLegalEntity.LegalEntity.Source == OrganisationType.CompaniesHouse ? "Companies House" :
                    accountLegalEntity.LegalEntity.Source == OrganisationType.Charities ? "Charities" :
                    accountLegalEntity.LegalEntity.Source == OrganisationType.PublicBodies ? "Public Bodies" :
                    accountLegalEntity.LegalEntity.Source == OrganisationType.PensionsRegulator ? "Pensions Regulator" : "Other",
                SourceNumeric = (short)accountLegalEntity.LegalEntity.Source,
            };
        }

        private async Task SetEmailAddressForSignatures(LegalEntity legalEntity)
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
}