using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountTeamMembers
{
    public class GetAccountTeamMembersResponse
    {
        public List<TeamMember> TeamMembers { get; set; }
    }
}