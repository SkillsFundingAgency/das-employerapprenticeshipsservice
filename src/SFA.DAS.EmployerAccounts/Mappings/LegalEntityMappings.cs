﻿using System.Linq;
using AutoMapper;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Models.Account;
using AccountLegalEntity = SFA.DAS.EmployerAccounts.Models.Account.AccountLegalEntity;

namespace SFA.DAS.EmployerAccounts.Mappings
{
    public class LegalEntityMappings : Profile
    {
        public LegalEntityMappings()
        {
            string accountHashedId = null;

            CreateMap<AccountSpecificLegalEntity, AccountSpecificLegalEntityDto>()
                .ForMember(d => d.RegisteredAddress, o => o.MapFrom(l => l.Address));

            CreateMap<AccountLegalEntity, AccountSpecificLegalEntityDto>()
                .ForMember(d => d.RegisteredAddress, o => o.MapFrom(l => l.Address))
                .ForMember(d => d.Id, o => o.MapFrom(l => l.LegalEntityId))
                .ForMember(d => d.DateOfIncorporation, o => o.MapFrom(l => l.LegalEntity.DateOfIncorporation))
                .ForMember(d => d.Code, o => o.MapFrom(l => l.LegalEntity.Code))
                .ForMember(d => d.Sector, o => o.MapFrom(l => l.LegalEntity.Sector))
                .ForMember(d => d.Status, o => o.MapFrom(l => l.LegalEntity.Status))
                .ForMember(d => d.AccountLegalEntityId, o => o.MapFrom(l => l.Id))
                .ForMember(d => d.PublicSectorDataSource, o => o.MapFrom(l => l.LegalEntity.PublicSectorDataSource))
                .ForMember(d => d.Source, o => o.MapFrom(l => l.LegalEntity.Source))
                .ForMember(d => d.AccountLegalEntityId, o => o.MapFrom(l => l.Id))
                .ForMember(d => d.AccountLegalEntityPublicHashedId, o => o.MapFrom(l => l.PublicHashedId));

            CreateMap<AccountLegalEntity, Api.Types.AccountLegalEntity>()
                .ForMember(d => d.AccountLegalEntityId, o => o.MapFrom(l => l.Id))
                .ForMember(d => d.AccountLegalEntityPublicHashedId, o => o.MapFrom(l => l.PublicHashedId));

            CreateMap<AccountLegalEntity, Api.Types.LegalEntity>()
                .ForMember(d => d.Agreements, o => o.MapFrom(l => l.Agreements.Where(a =>
                    (  a.StatusId == Models.EmployerAgreement.EmployerAgreementStatus.Pending ||
                        a.StatusId == Models.EmployerAgreement.EmployerAgreementStatus.Signed ))))
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
}