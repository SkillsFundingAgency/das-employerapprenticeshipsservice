namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class AccessDeniedViewModel: IAccountIdentifier
{
    public EmployerAccounts.Models.Account.Account Account { get; set; }
    public bool IsUser { get; set; }
    public long Id { get; set; }
    public long AccountId { get; set; }
    public string HashedAccountId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}