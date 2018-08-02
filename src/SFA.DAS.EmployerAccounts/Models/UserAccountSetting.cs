namespace SFA.DAS.EmployerAccounts.Models
{
    public class UserAccountSetting
    {
        public virtual long Id { get; protected set; }
        public virtual long AccountId { get; protected set; }
        public virtual long UserId { get; protected set; }
        public virtual Account Account { get; protected set; }
        public virtual User User { get; protected set; }
        public virtual bool ReceiveNotifications { get; protected set; }

        public UserAccountSetting(Account account, User user, bool receiveNotifications)
        {
            Account = account;
            User = user;
            ReceiveNotifications = receiveNotifications;
        }
    }
}