using SFA.DAS.EAS.Domain.Models;

namespace SFA.DAS.EAS.Support.Web.Models;

public class ChangeRoleViewModel
{
    public string HashedAccountId { get; set; }
    public string TeamMemberUserRef { get; set; }
    public Role Role { get; set; }
    public string AccountUri { get; set; }
}