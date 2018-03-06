using AutoMapper;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Domain.Models.Authorization;

namespace SFA.DAS.EAS.Application.Mappings
{
    public class AccountMappings : Profile
    {
        public AccountMappings()
        {
            CreateMap<Domain.Data.Entities.Account.Account, AccountContext>();
            CreateMap<Domain.Data.Entities.Account.Account, AccountDto>();
        }
    }
}