using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;

namespace SFA.DAS.EmployerAccounts.Queries.GetTeamMembersWhichReceiveNotifications
{
    public class GetTeamMembersWhichReceiveNotificationsQueryResponse
    {
        public List<TeamMember> TeamMembersWhichReceiveNotifications { get; set; }
    }
}
