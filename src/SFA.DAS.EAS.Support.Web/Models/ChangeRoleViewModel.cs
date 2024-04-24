using SFA.DAS.EAS.Domain.Models;

namespace SFA.DAS.EAS.Support.Web.Models;

public class ChangeRoleViewModel
{
    public string HashedAccountId { get; set; }
    public string UserRef { get; set; }
    public string Name { get; set; }
    public Role Role { get; set; }
    public string PostbackUrl { get; set; }
}