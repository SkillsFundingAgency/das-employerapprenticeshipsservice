using AutoMapper;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Account.Api.Mappings
{
    public class UserMappings : Profile
    {
        public UserMappings()
        {
            CreateMap<TeamMember, TeamMemberViewModel>();
        }
    }
}