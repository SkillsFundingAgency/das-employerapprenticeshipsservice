using System.Collections.Generic;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Web.Models
{
    public class EmployerTeamMembersViewModel
    {
        public List<TeamMember> TeamMembers { get; set; }
        public string HashedAccountId { get; set; }
        public string SuccessMessage { get; set; }
    }
}