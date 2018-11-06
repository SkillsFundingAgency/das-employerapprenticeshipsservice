using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;

namespace SFA.DAS.EmployerAccounts.Queries.GetTeamMembers
{
    public class GetTeamMembersResponse 
    {
        public ICollection<TeamMember> TeamMembers { get; set; }
    }
}
