using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EmployerAccounts.AcceptanceTests.Extensions
{
    public static class AccountExtensions
    {
        public static string GetDefaultLegalEntityCode(this Account account)
        {
            return $"{account.Name}-LE";
        }
    }
}
