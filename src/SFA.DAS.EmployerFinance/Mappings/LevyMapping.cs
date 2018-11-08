using AutoMapper;
using SFA.DAS.EmployerFinance.Models.Levy;

namespace SFA.DAS.EmployerFinance.Mappings
{
    public class LevyMapping : Profile
    {
        public LevyMapping()
        {
            CreateMap<LevyDeclarationView, LevyDeclarationViewModel>()
                .ForMember(target => target.PayeSchemeReference, opt => opt.MapFrom(src => src.EmpRef))
                .ForMember(target => target.HashedAccountId, opt => opt.Ignore());
        }
    }
}
