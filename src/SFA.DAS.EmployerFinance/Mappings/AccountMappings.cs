using AutoMapper;
using SFA.DAS.EmployerFinance.Dtos;
using SFA.DAS.EmployerFinance.Models.Account;

namespace SFA.DAS.EmployerFinance.Mappings
{
    public class AccountMappings : Profile
    {
        public AccountMappings()
        {
            CreateMap<Account, AccountDto>()
                .ForMember(m => m.HashedId, o => o.ResolveUsing<HashedResolver, long>(i => i.Id))
                .ForMember(m => m.PublicHashedId, o => o.ResolveUsing<PublicHashedResolver, long>(i => i.Id));
        }
    }
}