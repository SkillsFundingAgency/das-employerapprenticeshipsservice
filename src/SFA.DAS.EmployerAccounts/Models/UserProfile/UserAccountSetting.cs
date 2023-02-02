namespace SFA.DAS.EmployerAccounts.Models.UserProfile;

public class UserAccountSetting
{
    public virtual long Id { get; protected set; }
    public virtual long AccountId { get; protected set; }
    public virtual long UserId { get; protected set; }
    public virtual Account.Account Account { get; protected set; }
    public virtual User User { get; protected set; }
    public virtual bool ReceiveNotifications { get; protected set; }

    private UserAccountSetting() { }

    public UserAccountSetting(Account.Account account, User user, bool receiveNotifications)
    {
        Account = account;
        User = user;
        ReceiveNotifications = receiveNotifications;
    }
}