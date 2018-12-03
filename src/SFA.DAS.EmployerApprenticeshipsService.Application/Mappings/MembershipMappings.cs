using AutoMapper;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Application.Mappings
{
    public class MembershipMappings : Profile
    {
        public MembershipMappings()
        {
            CreateMap<Membership, MembershipContext>();
        }
    }
}