using AutoMapper;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EmployerAccounts.Api.Mappings
{
    public class EmployerAgreementViewMappings : Profile
    {
        public EmployerAgreementViewMappings()
        {
            CreateMap<Models.EmployerAgreement.EmployerAgreementView, EmployerAgreementView>();
        }
    }
}