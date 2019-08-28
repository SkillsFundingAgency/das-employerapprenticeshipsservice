using AutoMapper;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EmployerAccounts.Api.Mappings
{
    public class AccountMappings : Profile
    {
        public AccountMappings()
        {
            CreateMap<Models.EmployerAgreement.EmployerAgreementView, EmployerAgreementView>();
        }
    }
}