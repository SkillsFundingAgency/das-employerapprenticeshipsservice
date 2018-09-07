using SFA.DAS.Authorization;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class InviteTeamMemberViewModel :ViewModelBase
    {
        public string HashedAccountId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ExistingMemberError => GetErrorMessage("ExistingMember");
        public Role Role { get; set; }
        public string EmailError => GetErrorMessage(nameof(Email));
        public string NameError => GetErrorMessage(nameof(Name));
        public string RoleError => GetErrorMessage("RoleId");
    }
}