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

            CreateMap<LegalEntity, LegalEntityDto>();

            CreateMap<LegalEntity, LegalEntityViewModel>()
                .ForMember(d => d.Agreements, o => o.MapFrom(l => l.Agreements.Where(a =>
                    a.Account.Id == accountId && (
                    a.StatusId == EmployerAgreementStatus.Pending ||
                    a.StatusId == EmployerAgreementStatus.Signed))))
                .ForMember(d => d.Address, o => o.MapFrom(l => l.RegisteredAddress))
                .ForMember(d => d.DasAccountId, o => o.MapFrom(l => accountHashedId))
                .ForMember(d => d.DateOfInception, o => o.MapFrom(l => l.DateOfIncorporation))
                .ForMember(d => d.LegalEntityId, o => o.MapFrom(l => l.Id))
                .ForMember(d => d.PublicSectorDataSource, o => o.MapFrom(l =>
                    l.PublicSectorDataSource == 1 ? "ONS" :
                    l.PublicSectorDataSource == 2 ? "NHS" :
                    l.PublicSectorDataSource == 3 ? "Police" : ""))
                .ForMember(d => d.Source, o => o.MapFrom(l =>
                    l.Source == 1 ? "Companies House" :
                    l.Source == 2 ? "Charities" :
                    l.Source == 3 ? "Public Bodies" : "Other"))
                .ForMember(d => d.SourceNumeric, o => o.MapFrom(l => l.Source));
        }
    }
}