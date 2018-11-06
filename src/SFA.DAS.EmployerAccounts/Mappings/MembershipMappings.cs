using AutoMapper;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;

namespace SFA.DAS.EmployerAccounts.Mappings
{
    public class MembershipMappings : Profile
    {
        public MembershipMappings()
        {
            CreateMap<Membership, MembershipContext>();
            CreateMap<TeamMember, TeamMemberViewModel>();
        }
    }
}