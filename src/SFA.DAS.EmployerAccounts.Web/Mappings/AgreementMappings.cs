using AutoMapper;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreement;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Mappings
{
    public class AgreementMappings : Profile
    {
        public AgreementMappings()
        {
            CreateMap<AgreementDto, EmployerAccounts.Models.EmployerAgreement.EmployerAgreementView>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.StatusId))
                .ForMember(dest => dest.LegalEntityAddress, opt => opt.MapFrom(src => src.LegalEntity.RegisteredAddress))
                .ForMember(dest => dest.LegalEntityInceptionDate, opt => opt.MapFrom(src => src.LegalEntity.DateOfIncorporation))
                .ForMember(dest => dest.Sector, opt => opt.MapFrom(src => src.LegalEntity.Sector))
                .ForMember(dest => dest.LegalEntitySource, opt => opt.MapFrom(src => src.LegalEntity.Source))
                .ForMember(dest => dest.TemplatePartialViewName, opt => opt.MapFrom(src => src.Template.PartialViewName))
                .ForMember(dest => dest.AgreementType, opt => opt.MapFrom(src => src.Template.AgreementType))
                .ForMember(dest => dest.AccountLegalEntityId, opt => opt.MapFrom(src => src.LegalEntity.AccountLegalEntityId))
                .ForMember(dest => dest.AccountLegalEntityPublicHashedId, opt => opt.MapFrom(src => src.LegalEntity.AccountLegalEntityPublicHashedId))
                .ForMember(dest => dest.AgreementType, opts => opts.Ignore())
                .ForMember(dest => dest.VersionNumber, opts => opts.Ignore());

            CreateMap<GetEmployerAgreementResponse, EmployerAgreementViewModel>()
                .ForMember(dest => dest.PreviouslySignedEmployerAgreement, opt => opt.MapFrom(src => src.LastSignedAgreement))
                .ForMember(dest => dest.OrganisationLookupPossible, opt => opt.Ignore())
                .ForMember(dest => dest.NoChoiceSelected, opts => opts.Ignore());

            CreateMap<AccountDetailViewModel, AgreementInfoViewModel>()
                .ConvertUsing(new AgreementInfoConverter());
        }
    }
}