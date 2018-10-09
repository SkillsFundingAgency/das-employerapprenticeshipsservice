using AutoMapper;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerFinance.Dtos;
using SFA.DAS.EmployerFinance.Models.Account;

namespace SFA.DAS.EmployerFinance.Mappings
{
    public class AccountMappings : Profile
    {
        public AccountMappings()
        {
            CreateMap<Account, AccountContext>();

            CreateMap<Account, AccountDto>()
                .ForMember(d => d.CreatedDate, o => o.Ignore())
                .ForMember(d => d.ModifiedDate, o => o.Ignore());
        }
    }
}