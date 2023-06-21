using System.Linq;
using AutoMapper;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Domain.Models.Account;
using EmployerAgreementStatus = SFA.DAS.EAS.Domain.Models.EmployerAgreement.EmployerAgreementStatus;

namespace SFA.DAS.EAS.Application.Mappings;

public class LegalEntityMappings : Profile
{
    public LegalEntityMappings()
    {
        long accountId = 0;
        string accountHashedId = null;
      
        CreateMap<AccountLegalEntity, LegalEntityViewModel>()
            .ForMember(d => d.Agreements, o => o.MapFrom(l => l.Agreements.Where(a =>
                a.AccountLegalEntity.AccountId == accountId && (
                    a.StatusId == EmployerAgreementStatus.Pending ||
                    a.StatusId == EmployerAgreementStatus.Signed))))
            .ForMember(d => d.DasAccountId, o => o.MapFrom(l => accountHashedId))
            .ForMember(d => d.AccountLegalEntityId, o => o.MapFrom(l => l.Id))
            .ForMember(dest => dest.AccountLegalEntityPublicHashedId, o => o.MapFrom(l => l.PublicHashedId))
            .ForMember(d => d.LegalEntityId, o => o.MapFrom(l => l.LegalEntityId))
            .ForMember(d => d.DateOfInception, o => o.MapFrom(l => l.LegalEntity.DateOfIncorporation))
            .ForMember(d => d.Code, o => o.MapFrom(l => l.LegalEntity.Code))
            .ForMember(d => d.Sector, o => o.MapFrom(l => l.LegalEntity.Sector))
            .ForMember(d => d.Status, o => o.MapFrom(l => l.LegalEntity.Status))
            .ForMember(d => d.PublicSectorDataSource, o => o.MapFrom(l =>
                l.LegalEntity.PublicSectorDataSource == 1 ? "ONS" :
                l.LegalEntity.PublicSectorDataSource == 2 ? "NHS" :
                l.LegalEntity.PublicSectorDataSource == 3 ? "Police" : ""))
            .ForMember(d => d.Source, o => o.MapFrom(l =>
                l.LegalEntity.Source == OrganisationType.CompaniesHouse ? "Companies House" :
                l.LegalEntity.Source == OrganisationType.Charities ? "Charities" :
                l.LegalEntity.Source == OrganisationType.PublicBodies ? "Public Bodies" :
                l.LegalEntity.Source == OrganisationType.PensionsRegulator ? "Pensions Regulator" : "Other"))
            .ForMember(d => d.SourceNumeric, o => o.MapFrom(l => (short) l.LegalEntity.Source))
            .ForMember(d => d.AgreementSignedByName, o => o.Ignore())
            .ForMember(d => d.AgreementSignedDate, o => o.Ignore())
            .ForMember(d => d.AgreementStatus, o => o.Ignore());
    }
}