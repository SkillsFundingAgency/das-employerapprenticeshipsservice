using AutoMapper;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerFinance.Dtos;
using SFA.DAS.EmployerFinance.Models.Account;

namespace SFA.DAS.EmployerFinance.Mappings
{
    public class AccountMappings : Profile
    {
        public AccountMappings()
        {
            CreateMap<Account, AccountDto>();

            CreateMap<AccountDetailViewModel, AccountDetailDto>()
                .ForMember(m => m.HashedId, o => o.MapFrom(i => i.HashedAccountId))
                .ForMember(m => m.PublicHashedId, o => o.MapFrom(i => i.PublicHashedAccountId));
        }
    }
}