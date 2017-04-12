using AutoMapper;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Api.Mappings
{
    public class AccountMappings : Profile
    {
        public AccountMappings()
        {
            CreateMap<Domain.Data.Entities.Account.Account, AccountDetailViewModel>()
                .ForMember(target => target.HashedAccountId, opt => opt.MapFrom(src => src.HashedId))
                .ForMember(target => target.DasAccountName, opt => opt.MapFrom(src => src.Name));
        }
    }
}