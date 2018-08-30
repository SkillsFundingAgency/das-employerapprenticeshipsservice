using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EmployerAccounts.AcceptanceTests.Extensions
{
    public static class MembershipExtensions
    {
        public static string GetMembershipKey(this Membership membership)
        {
            return $"{membership.User}{membership.Account}";
        }

    }
}
