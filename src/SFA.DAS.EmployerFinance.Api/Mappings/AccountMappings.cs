using AutoMapper;
using SFA.DAS.EmployerFinance.Api.Types;
using SFA.DAS.EmployerFinance.Models.Levy;

namespace SFA.DAS.EmployerFinance.Api.Mappings
{
    public class AccountMappings : Profile
    {
        public AccountMappings()
        {            
            CreateMap<LevyDeclarationItem, LevyDeclaration>()
               .ForMember(target => target.PayeSchemeReference, opt => opt.MapFrom(src => src.EmpRef));

            CreateMap<Models.Account.AccountBalance, AccountBalance>();               
        }
    }
}