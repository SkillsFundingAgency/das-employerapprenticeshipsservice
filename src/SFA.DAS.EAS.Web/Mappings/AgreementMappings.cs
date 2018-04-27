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
                .ForMember(dest => dest.TemplatePartialViewName, opt => opt.MapFrom(src => src.Template.PartialViewName));

            CreateMap<GetEmployerAgreementResponse, EmployerAgreementViewModel>();
        }
    }
}