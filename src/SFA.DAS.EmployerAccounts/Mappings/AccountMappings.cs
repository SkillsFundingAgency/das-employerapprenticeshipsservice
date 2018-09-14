using AutoMapper;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Mappings
{
    public class AccountMappings : Profile
    {
        public AccountMappings()
        {
            CreateMap<Account, AccountContext>();
            CreateMap<Account, AccountDto>();
        }
    }
}