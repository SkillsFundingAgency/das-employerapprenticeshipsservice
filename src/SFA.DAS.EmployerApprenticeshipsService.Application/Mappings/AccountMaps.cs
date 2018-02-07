using AutoMapper;
using SFA.DAS.EAS.Application.Dtos;

namespace SFA.DAS.EAS.Application.Mappings
{
    public class AccountMaps : Profile
    {
        public AccountMaps()
        {
            CreateMap<Domain.Data.Entities.Account.Account, AccountDto>();
        }
    }
}