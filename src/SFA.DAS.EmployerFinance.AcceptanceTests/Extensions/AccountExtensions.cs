using SFA.DAS.EmployerFinance.Models.Account;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Extensions
{
    public static class AccountExtensions
    {
        
        public static string GetDefaultLegalEntityCode(this Account account)
        {
            return $"{account.Name}-LE";
        }
    }
}
