using AutoMapper;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

namespace SFA.DAS.EmployerAccounts.Mappings;

public class AgreementMappings : Profile
{
    public AgreementMappings()
    {
        CreateMap<AgreementTemplate, AgreementTemplateDto>();

        CreateMap<EmployerAgreement, AgreementDto>()
            .ForMember(
                d => d.AccountId,
                conf => conf.MapFrom(d => d.AccountLegalEntity.AccountId))
            .ForMember(
                d => d.Account,
                conf => conf.MapFrom(d => d.AccountLegalEntity.Account))
            .ForMember(
                d => d.LegalEntityId,
                conf => conf.MapFrom(d => d.AccountLegalEntity.LegalEntityId))
            .ForMember(
                d => d.LegalEntity,
                conf => conf.MapFrom(d => d.AccountLegalEntity))
            .ForMember(
                d => d.HashedAccountId,
                o => o.Ignore())
            .ForMember(
                d => d.HashedAgreementId,
                o => o.Ignore());

        CreateMap<EmployerAgreement, AgreementViewModel>()
            .ForMember(
                v => v.Status,
                o => o.MapFrom(a => (EmployerAgreementStatus) (int) a.StatusId))
            .ForMember(
                v => v.TemplateVersionNumber,
                o => o.MapFrom(a => a.Template.VersionNumber));

        CreateMap<EmployerAgreement, Api.Types.Agreement>()
            .ForMember(
                v => v.Status,
                o => o.MapFrom(a => (Api.Types.EmployerAgreementStatus) (int) a.StatusId))
            .ForMember(
                v => v.TemplateVersionNumber,
                o => o.MapFrom(a => a.Template.VersionNumber))
            .ForMember(
                v => v.AgreementType,
                o => o.MapFrom(a => a.Template.AgreementType))
            .ForMember(
                d => d.SignedByEmail, 
                o => o.Ignore());

        CreateMap<EmployerAgreement, EmployerAgreementDto>()
            .ForMember(
                d => d.OrganisationLookupPossible,
                o => o.Ignore())               
            .ForMember(
                d => d.HashedAccountId,
                o => o.Ignore())
            .ForMember(
                d => d.HashedAgreementId,
                o => o.Ignore())
            .ForMember(
                d => d.HashedLegalEntityId,
                o => o.Ignore());

        CreateMap<AccountLegalEntity, AccountLegalEntityDto>();
    }
}