namespace SFA.DAS.EmployerAccounts.Models.Account;

public class AccountHistory
{
    public long Id { get; set; }
    public long AccountId { get; set; }
    public string PayeRef { get; set; }
    public DateTime AddedDate { get; set; }
    public DateTime? RemovedDate { get; set; }
}