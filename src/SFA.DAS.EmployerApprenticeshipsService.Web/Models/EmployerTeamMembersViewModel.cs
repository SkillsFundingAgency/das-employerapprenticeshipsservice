using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class EmployerTeamMembersViewModel
    {
        public List<TeamMember> TeamMembers { get; set; }
        public long AccountId { get; set; }
        public string SuccessMessage { get; set; }
    }
}