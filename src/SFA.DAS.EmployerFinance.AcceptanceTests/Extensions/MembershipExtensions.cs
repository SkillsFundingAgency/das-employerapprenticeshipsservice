using SFA.DAS.EmployerFinance.Models.AccountTeam;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Extensions
{
    public static class MembershipExtensions
    {
        public static string GetMembershipKey(this Membership membership)
        {
            return $"{membership.User.FullName}{membership.Account.Name}";
        }
    }
}
