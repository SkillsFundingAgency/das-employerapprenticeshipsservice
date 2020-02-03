using SFA.DAS.EmployerAccounts.Interfaces;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class AccountSummaryViewModel : IAccountIdentifier
    {
        public EmployerAccounts.Models.Account.Account Account { get; set; }
        public string HashedAccountId { get; set; }
    }
}