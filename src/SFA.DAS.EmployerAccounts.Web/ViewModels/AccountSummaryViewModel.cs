namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class AccountSummaryViewModel : IAccountIdentifier
{
    public Account Account { get; set; }
    public string HashedAccountId { get; set; }
}