using AutoMapper;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Mappings;

public class EmploymentAgreementStatusMappings : Profile
{
    public EmploymentAgreementStatusMappings()
    {
        CreateMap<EmployerAgreement, SignedEmployerAgreementDetailsDto>()
            .ForMember(d => d.HashedAgreementId, o => o.Ignore())
            .ForMember(d => d.PartialViewName, conf => conf.MapFrom(ol => ol.Template.PartialViewName))
            .ForMember(d => d.TemplateId, conf => conf.MapFrom(ol => ol.Template.Id))
            .ForMember(d => d.VersionNumber, conf => conf.MapFrom(ol => ol.Template.VersionNumber));

        CreateMap<EmployerAgreement, EmployerAgreementDetailsDto>()
            .ForMember(d => d.HashedAgreementId, o => o.Ignore())
            .ForMember(d => d.PartialViewName, conf => conf.MapFrom(ol => ol.Template.PartialViewName))
            .ForMember(d => d.TemplateId, conf => conf.MapFrom(ol => ol.Template.Id))
            .ForMember(d => d.VersionNumber, conf => conf.MapFrom(ol => ol.Template.VersionNumber));

        CreateMap<AccountLegalEntity, EmployerAgreementStatusDto>()
            .ForMember(d => d.LegalEntity, o => o.MapFrom(g => g.LegalEntity))
            .ForMember(d => d.AccountId, o => o.Ignore())
            .ForMember(d => d.HashedAccountId, o => o.Ignore())
            .ForMember(d => d.Signed, o => o.MapFrom(g => g.SignedAgreement))
            .ForMember(d => d.Pending, o => o.MapFrom(g => g.PendingAgreement))
            .ForMember(d => d.LegalEntity, o => o.MapFrom(l => l));
    }
}