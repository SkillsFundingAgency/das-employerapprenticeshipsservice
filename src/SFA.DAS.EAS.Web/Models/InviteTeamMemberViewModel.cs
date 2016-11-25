using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Web.Models
{
    public class InviteTeamMemberViewModel :ViewModelBase
    {
        public string HashedId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ExistingMemberError => GetErrorMessage("ExistingMember");
        public Role Role { get; set; }
        public string EmailError => GetErrorMessage(nameof(Email));
        public string NameError => GetErrorMessage(nameof(Name));
        public string RoleError => GetErrorMessage("RoleId");
    }
}