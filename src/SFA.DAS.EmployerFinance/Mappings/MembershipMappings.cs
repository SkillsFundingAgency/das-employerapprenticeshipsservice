using AutoMapper;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerFinance.Models.AccountTeam;

namespace SFA.DAS.EmployerFinance.Mappings
{
    public class MembershipMappings : Profile
    {
        public MembershipMappings()
        {
            CreateMap<Membership, MembershipContext>();
        }
    }
}