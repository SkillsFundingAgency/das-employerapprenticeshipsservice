using SFA.DAS.EmployerAccounts.Models.AccountTeam;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class EmployerTeamMembersViewModel : IAccountIdentifier
{
    public List<TeamMember> TeamMembers { get; set; }
    public string HashedAccountId { get; set; }
    public string SuccessMessage { get; set; }
}