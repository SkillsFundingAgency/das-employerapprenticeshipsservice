using AutoMapper;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;

namespace SFA.DAS.EmployerAccounts.Mappings
{
    public class MembershipMappings : Profile
    {
        public MembershipMappings()
        {
            CreateMap<TeamMember, Api.Types.TeamMember>();
        }
    }
}