namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class InviteTeamMemberViewModel : ViewModelBase , IAccountIdentifier
{
    public string HashedAccountId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string ExistingMemberError => GetErrorMessage("ExistingMember");
    public Role Role { get; set; }
    public string EmailError => GetErrorMessage(nameof(Email));
    public string NameError => GetErrorMessage(nameof(Name));
    public string RoleError => GetErrorMessage(nameof(Role));
}