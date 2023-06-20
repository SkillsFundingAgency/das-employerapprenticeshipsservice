using SFA.DAS.EmployerAccounts.Models.AccountTeam;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class UserInvitationsViewModel
{
    public List<InvitationView> Invitations { get; set; }
    public bool ShowBreadCrumbs { get; set; }
}