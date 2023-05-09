namespace SFA.DAS.EAS.Domain.Models.Account;

public class Accounts<T>
{
    public int AccountsCount { get; set; }
    public List<T> AccountList { get; set; }
}
