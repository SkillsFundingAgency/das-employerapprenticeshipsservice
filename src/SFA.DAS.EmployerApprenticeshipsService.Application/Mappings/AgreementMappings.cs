using AutoMapper;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Application.Mappings
{
    public class AgreementMappings : Profile
    {
        public AgreementMappings()
        {
            CreateMap<AgreementTemplate, AgreementTemplateDto>();

            CreateMap<EmployerAgreement, AgreementDto>()
                .ForMember(d => d.HashedAccountId, o => o.Ignore())
                .ForMember(d => d.HashedAgreementId, o => o.Ignore());

            CreateMap<EmployerAgreement, AgreementViewModel>()
                .ForMember(v => v.Status, o => o.MapFrom(a => (EmployerAgreementStatus)(int)a.StatusId));
        }
    }
}