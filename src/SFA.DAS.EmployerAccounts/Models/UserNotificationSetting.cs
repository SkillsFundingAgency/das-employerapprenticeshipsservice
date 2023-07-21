namespace SFA.DAS.EmployerAccounts.Models;

public class UserNotificationSetting
{
    public long Id { get; set; }
    public long AccountId { get; set; }
    public string HashedAccountId { get; set; }
    public long UserId { get; set; }
    public string Name { get; set; }
    public bool ReceiveNotifications { get; set; }
}