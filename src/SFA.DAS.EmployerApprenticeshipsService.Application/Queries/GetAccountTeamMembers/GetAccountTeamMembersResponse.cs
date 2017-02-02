using System.Collections.Generic;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Application.Queries.GetAccountTeamMembers
{
    public class GetAccountTeamMembersResponse
    {
        public List<TeamMember> TeamMembers { get; set; }
    }
}