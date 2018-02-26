using AutoMapper;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.Mappings
{
    public class UserMaps : Profile
    {
        public UserMaps()
        {
            CreateMap<User, UserDto>();
        }
    }
}