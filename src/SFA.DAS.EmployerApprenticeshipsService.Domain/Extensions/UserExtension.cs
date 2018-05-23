using System.Linq;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Domain.Extensions
{
    public static class UserExtension
    {
        public static IQueryable<User> UsersThatReceiveNotifications(this IQueryable<User> users, long accountId)
        {
            return users
                .Where(user =>
                    user.UserAccountSettings.Any(us =>
                        us.AccountId == accountId && us.ReceiveNotifications) &&
                    user.Memberships.Any(ms =>
                        ms.AccountId == accountId && ms.Role == Role.Owner));

        }
    }
}
