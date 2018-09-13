using AutoMapper;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerFinance.Dtos;

namespace SFA.DAS.EmployerFinance.Mappings
{
    public class AccountMappings : Profile
    {
        public AccountMappings()
        {
            CreateMap<Models.Account.Account, AccountContext>();
            CreateMap<Models.Account.Account, AccountDto>();
        }
    }
}