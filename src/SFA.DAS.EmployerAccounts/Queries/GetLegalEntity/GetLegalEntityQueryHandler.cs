using MediatR;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.Account;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
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

                    .Select(x => new LegalEntity
                    {
                        Agreements = MapAgreements(x.Agreements, message.IncludeAllAgreements),
                        DasAccountId = message.AccountHashedId,
                        AccountLegalEntityId = x.Id,
                        AccountLegalEntityPublicHashedId = x.PublicHashedId,
                        LegalEntityId = x.LegalEntityId,
                        DateOfInception = x.LegalEntity.DateOfIncorporation,
                        Code = x.LegalEntity.Code,
                        Sector = x.LegalEntity.Sector,
                        Status = x.LegalEntity.Status,
                        PublicSectorDataSource = MapPublicSectorDataSource(x.LegalEntity.PublicSectorDataSource),
                        Source = MapSource(x.LegalEntity.Source),
                        SourceNumeric = (short)x.LegalEntity.Source,
                    }).SingleOrDefault();


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

        private static string MapSource(OrganisationType legalEntitySource)
        {
            switch (legalEntitySource)
            {
                case OrganisationType.CompaniesHouse:
                    return "Companies House";
                case OrganisationType.Charities:
                    return "Charities";
                case OrganisationType.PublicBodies:
                    return "Public Bodies";
                case OrganisationType.PensionsRegulator:
                    return "Pensions Regulator";
                default:
                    return "Other";
            }
        }

        private static string MapPublicSectorDataSource(byte? publicSectorDataSource)
        {
            switch (publicSectorDataSource)
            {
                case 1:
                    return "ONS";
                case 2:
                    return "NHS";
                case 3:
                    return "Police";
                default:
                    return "";
            }
        }

        private static List<Agreement> MapAgreements(IEnumerable<EmployerAgreement> agreements, bool includeAllAgreements)
        {
            return agreements.Where(x =>
                includeAllAgreements || (x.StatusId == Models.EmployerAgreement.EmployerAgreementStatus.Pending ||
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

                })
                .ToList();
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