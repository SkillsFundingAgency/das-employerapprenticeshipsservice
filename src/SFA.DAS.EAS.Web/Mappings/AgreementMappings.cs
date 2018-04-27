using AutoMapper;
using SFA.DAS.EAS.Application.Dtos.EmployerAgreement;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreement;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Mappings
{
    public class AgreementMappings : Profile
    {
        public AgreementMappings()
        {
            CreateMap<EmployerAgreementDto, EmployerAgreementView>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.StatusId))
                .ForMember(dest => dest.LegalEntityAddress, opt => opt.MapFrom(src => src.LegalEntity.RegisteredAddress))
                .ForMember(dest => dest.LegalEntityInceptionDate, opt => opt.MapFrom(src => src.LegalEntity.DateOfIncorporation))
                .ForMember(dest => dest.Sector, opt => opt.MapFrom(src => src.LegalEntity.Sector))
                .ForMember(dest => dest.TemplatePartialViewName, opt => opt.MapFrom(src => src.Template.PartialViewName));

            CreateMap<GetEmployerAgreementResponse, EmployerAgreementViewModel>()
                .ForMember(dest => dest.PreviouslySignedEmployerAgreement, opt => opt.MapFrom(src => src.LastSignedAgreement));
        }
    }
}