using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EAS.Support.Web.Models;

[ExcludeFromCodeCoverage]
public class AccountDetailViewModel
{
    public Core.Models.Account Account { get; set; }
    public string SearchUrl { get; set; }
    public string AccountUri { get; set; }
    public bool IsTier2User { get; set; }
    public string ChangeRoleUrl { get; set; }
    public string ResendInviteUrl { get; set; }
}