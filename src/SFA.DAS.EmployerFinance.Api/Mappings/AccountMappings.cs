using AutoMapper;
using SFA.DAS.EAS.Finance.Api.Types;

namespace SFA.DAS.EmployerFinance.Api.Mappings
{
    public class AccountMappings : Profile
    {
        public AccountMappings()
        {            
            CreateMap<DAS.EmployerFinance.Models.Levy.LevyDeclarationView, LevyDeclarationViewModel>()
               .ForMember(target => target.PayeSchemeReference, opt => opt.MapFrom(src => src.EmpRef));
        }
    }
}