using System.Collections.Generic;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Application.Queries.GetAccountTeamMembers
{
    public class GetAccountTeamMembersResponse
    {
        public List<TeamMember> TeamMembers { get; set; }
    }
}