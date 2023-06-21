using AutoMapper;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Application.Mappings;

public class AgreementMappings : Profile
{
    public AgreementMappings()
    {
        CreateMap<EmployerAgreement, AgreementViewModel>()
            .ForMember(v => v.Status, o => o.MapFrom(a => (EmployerAgreementStatus)(int)a.StatusId))
            .ForMember(v => v.TemplateVersionNumber, o => o.MapFrom(a => a.Template.VersionNumber))
            .ForMember(v => v.AgreementType, o => o.MapFrom(a => a.Template.AgreementType)
            );
    }
}