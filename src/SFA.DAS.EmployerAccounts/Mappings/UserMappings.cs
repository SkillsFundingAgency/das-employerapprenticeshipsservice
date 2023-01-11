using AutoMapper;
using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Mappings;

public class UserMappings : Profile
{
    public UserMappings()
    {
        CreateMap<User, UserDto>();
    }
}