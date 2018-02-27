using AutoMapper;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.Authorization;

namespace SFA.DAS.EAS.Application.Mappings
{
    public class MembershipMaps : Profile
    {
        public MembershipMaps()
        {
            CreateMap<Membership, MembershipContext>();
        }
    }
}