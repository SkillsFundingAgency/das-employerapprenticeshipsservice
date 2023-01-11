namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class InviteTeamMemberNextStepsViewModel : IAccountIdentifier
{
    public string ErrorMessage { get; set; }
    public bool UserShownWizard { get; set; }
    public string HashedAccountId { get; set; }
}