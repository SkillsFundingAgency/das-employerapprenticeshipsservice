using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class UserAccountsViewModel
    {
        public Accounts<Account> Accounts;
        public int Invitations;
        public FlashMessageViewModel FlashMessage;
        public string ErrorMessage;
    }
}
