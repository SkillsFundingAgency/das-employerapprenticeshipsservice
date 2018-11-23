using AutoMapper;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

namespace SFA.DAS.EmployerAccounts.Mappings
{
    public class AgreementMappings : Profile
    {
        public AgreementMappings()
        {
            CreateMap<EmployerAgreement, AgreementViewModel>()
                .ForMember(v => v.Status, o => o.MapFrom(a => (EmployerAgreementStatus) (int) a.StatusId))
                .ForMember(v => v.TemplateVersionNumber, o => o.MapFrom(a => a.Template.VersionNumber));
        }
    }
}