namespace SFA.DAS.EmployerAccounts.Models.Account;

public class Accounts<T>
{
    public int AccountsCount { get; set; }
    public List<T> AccountList { get; set; }
}