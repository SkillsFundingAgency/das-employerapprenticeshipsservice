using AutoMapper;
using SFA.DAS.EmployerFinance.Dtos;
using SFA.DAS.EmployerFinance.Models.Account;

namespace SFA.DAS.EmployerFinance.Mappings
{
    public class AccountMappings : Profile
    {
        public AccountMappings()
        {
            CreateMap<Account, AccountDto>();
        }
    }
}