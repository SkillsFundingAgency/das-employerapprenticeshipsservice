using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Application.Queries.GetTeamMembers
{
    public class GetTeamMembersResponse 
    {
        public ICollection<TeamMember> TeamMembers { get; set; }
    }
}
