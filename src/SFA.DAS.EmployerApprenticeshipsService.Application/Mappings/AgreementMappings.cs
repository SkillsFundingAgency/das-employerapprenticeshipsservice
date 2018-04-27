using AutoMapper;
using SFA.DAS.EAS.Application.Dtos.EmployerAgreement;
using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Application.Mappings
{
    public class AgreementMappings : Profile
    {
        public AgreementMappings()
        {
            CreateMap<EmployerAgreement, EmployerAgreementDto>();

            CreateMap<AgreementTemplate, AgreementTemplateDto>();
        }
    }
}