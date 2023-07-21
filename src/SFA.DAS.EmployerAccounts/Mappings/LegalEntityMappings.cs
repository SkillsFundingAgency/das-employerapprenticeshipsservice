using AutoMapper;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Models.Account;
using AccountLegalEntity = SFA.DAS.EmployerAccounts.Models.Account.AccountLegalEntity;

namespace SFA.DAS.EmployerAccounts.Mappings;

public class LegalEntityMappings : Profile
{
    public LegalEntityMappings()
    {
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

    }
}