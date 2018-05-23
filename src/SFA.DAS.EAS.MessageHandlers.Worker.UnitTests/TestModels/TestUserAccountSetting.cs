using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.MessageHandlers.Worker.UnitTests.TestModels
{
    internal sealed class TestUserAccountSetting : UserAccountSetting
    {
        public TestUserAccountSetting() { }
        public TestUserAccountSetting(Account account, User user,
            bool receiveNotifications)
        {
            Account = account;
            AccountId = account.Id;
            User = user;
            UserId = user.Id;
            ReceiveNotifications = receiveNotifications;
        }
    }
}