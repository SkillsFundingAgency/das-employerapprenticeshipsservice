using SFA.DAS.EAS.Domain.Models;

namespace SFA.DAS.EAS.Support.Web.Models;

public class InvitationViewModel
{
    public string Email { get; set; }
    public string HashedAccountId { get; set; }
    public string FullName { get; set; }
    public Role Role { get; set; } = Role.None;
    public string ResponseUrl { get; set; }
}