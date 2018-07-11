using System.Linq;
using AutoMapper;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Domain.Models.Account;
using EmployerAgreementStatus = SFA.DAS.EAS.Domain.Models.EmployerAgreement.EmployerAgreementStatus;

namespace SFA.DAS.EAS.Application.Mappings
{
    public class LegalEntityMappings : Profile
    {
        public LegalEntityMappings()
        {
            long accountId = 0;
            string accountHashedId = null;

            CreateMap<AccountSpecificLegalEntity, AccountSpecificLegalEntityDto>()
                .ForMember(d => d.RegisteredAddress, o => o.MapFrom(l => l.Address))
                .ForMember(d => d.HashedAccountLegalEntityId, o => o.MapFrom(l => l.AccountLegalEntityHashedId));

            CreateMap<AccountLegalEntity, AccountSpecificLegalEntityDto>()
                .ForMember(d => d.RegisteredAddress, o => o.MapFrom(l => l.Address))
                .ForMember(d => d.Id, o => o.MapFrom(l => l.LegalEntityId))
                .ForMember(d => d.DateOfIncorporation, o => o.MapFrom(l => l.LegalEntity.DateOfIncorporation))
                .ForMember(d => d.Code, o => o.MapFrom(l => l.LegalEntity.Code))
                .ForMember(d => d.Sector, o => o.MapFrom(l => l.LegalEntity.Sector))
                .ForMember(d => d.Status, o => o.MapFrom(l => l.LegalEntity.Status))
                .ForMember(d => d.PublicSectorDataSource, o => o.MapFrom(l => l.LegalEntity.PublicSectorDataSource))
                .ForMember(d => d.Source, o => o.MapFrom(l => l.LegalEntity.Source))
                .ForMember(d => d.HashedAccountLegalEntityId, o => o.MapFrom(l => l.PublicHashedId));

            CreateMap<AccountLegalEntity, LegalEntityViewModel>()
                .ForMember(d => d.Agreements, o => o.MapFrom(l => l.Agreements))
                //.ForMember(d => d.Agreements, o => o.MapFrom(l => l.Agreements.Where(a =>
                //        a.AccountLegalEntity.AccountId == accountId && (
                //            a.StatusId == EmployerAgreementStatus.Pending ||
                //            a.StatusId == EmployerAgreementStatus.Signed))))
                .ForMember(d => d.DasAccountId, o => o.MapFrom(l => accountHashedId))
                .ForMember(d => d.AccountLegalEntityId, o => o.MapFrom(l => l.Id))
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
                    l.LegalEntity.Source == 1 ? "Companies House" :
                    l.LegalEntity.Source == 2 ? "Charities" :
                    l.LegalEntity.Source == 3 ? "Public Bodies" : "Other"))
                .ForMember(d => d.SourceNumeric, o => o.MapFrom(l => l.LegalEntity.Source))
                // These obsolete items are populated explicitly by the 
                .ForMember(d => d.AgreementSignedByName, o => o.Ignore())
                .ForMember(d => d.AgreementSignedDate, o => o.Ignore())
                .ForMember(d => d.AgreementStatus, o => o.Ignore());
        }
    }
}