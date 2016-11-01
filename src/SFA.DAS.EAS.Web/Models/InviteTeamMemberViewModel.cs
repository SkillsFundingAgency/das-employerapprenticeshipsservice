using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Web.Models
{
    public class InviteTeamMemberViewModel
    {
        public string HashedId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
    }
}