using AutoMapper;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerFinance.Dtos;
using SFA.DAS.EmployerFinance.Models.UserProfile;

namespace SFA.DAS.EmployerFinance.Mappings
{
    public class UserMappings : Profile
    {
        public UserMappings()
        {
            CreateMap<User, UserContext>();
            CreateMap<User, UserDto>();
        }
    }
}