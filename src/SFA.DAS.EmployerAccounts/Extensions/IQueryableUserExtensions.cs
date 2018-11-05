using System.Linq;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Models.UserProfile;

namespace SFA.DAS.EmployerAccounts.Extensions
{
    public static class IQueryableUserExtensions
    {
        public static IQueryable<User> WhereReceiveNotifications(this IQueryable<User> users, long accountId)
        {
            return users.Where(u =>
                u.Memberships.Any(m => m.Account.Id == accountId && m.Role == Role.Owner &&  m.ReceiveNotifications));
        }
    }
}