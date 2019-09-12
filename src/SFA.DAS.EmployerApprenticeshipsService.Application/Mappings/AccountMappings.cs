using AutoMapper;
using SFA.DAS.EAS.Application.Dtos;

namespace SFA.DAS.EAS.Application.Mappings
{
    public class AccountMappings : Profile
    {
        public AccountMappings()
        {
            CreateMap<Domain.Models.Account.Account, AccountDto>();
        }
    }
}