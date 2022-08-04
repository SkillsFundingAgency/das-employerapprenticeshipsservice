using AutoMapper;
using SFA.DAS.EAS.Finance.Api.Types;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.EmployerFinance.Queries.GetLevyDeclaration;

namespace SFA.DAS.EmployerFinance.Api.Mappings
{
    public class AccountMappings : Profile
    {
        public AccountMappings()
        {            
            CreateMap<LevyDeclarationItem, LevyDeclaration>()
               .ForMember(target => target.PayeSchemeReference, opt => opt.MapFrom(src => src.EmpRef));
        }
    }
}