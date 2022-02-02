using System.Linq;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Models.Account;
using AccountLegalEntity = SFA.DAS.EmployerAccounts.Models.Account.AccountLegalEntity;
using LegalEntity = SFA.DAS.EmployerAccounts.Api.Types.LegalEntity;

namespace SFA.DAS.EmployerAccounts.Api.Mappings
{
    public static class LegalEntityMapping
    {
        public static LegalEntity MapFromAccountLegalEntity(AccountLegalEntity source, EmployerAgreement agreement, bool includeAllAgreements)
        {
            if (source == null)
            {
                return null;
            }
            
            var legalEntity =  new LegalEntity
            {
                Agreements = source.Agreements.Where(x =>
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
                        SignedDate = a.SignedDate,
                        SignedByEmail = a.SignedByEmail
                    }).ToList(),
                Address = source.Address,
                Name = source.Name,
                DasAccountId = source.Account.HashedId,
                AccountLegalEntityId = source.Id,
                AccountLegalEntityPublicHashedId = source.PublicHashedId,
                LegalEntityId = source.LegalEntityId,
                DateOfInception = source.LegalEntity.DateOfIncorporation,
                Code = source.LegalEntity.Code,
                Sector = source.LegalEntity.Sector,
                Status = source.LegalEntity.Status,
                PublicSectorDataSource = source.LegalEntity.PublicSectorDataSource == 1 ? "ONS" : source.LegalEntity.PublicSectorDataSource == 2 ? "NHS" : source.LegalEntity.PublicSectorDataSource == 3 ? "Police" : "",
                Source = source.LegalEntity.Source == OrganisationType.CompaniesHouse ? "Companies House" :
                    source.LegalEntity.Source == OrganisationType.Charities ? "Charities" :
                    source.LegalEntity.Source == OrganisationType.PublicBodies ? "Public Bodies" :
                    source.LegalEntity.Source == OrganisationType.PensionsRegulator ? "Pensions Regulator" : "Other",
                SourceNumeric = (short)source.LegalEntity.Source
            };
            
            //TODO : These are obsolete and have been for quite a while, and should be deleted.
            if (agreement != null)
            {
                legalEntity.AgreementSignedByName = agreement.SignedByName;
                legalEntity.AgreementSignedDate = agreement.SignedDate;
                legalEntity.AgreementStatus = (EmployerAgreementStatus)(int)agreement.StatusId;    
            }

            return legalEntity;
        }
    }
}