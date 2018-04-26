using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class UserAccountsViewModel
    {
        public Accounts<Account> Accounts;
        public int Invitations;
        public FlashMessageViewModel FlashMessage;
        public string ErrorMessage;
    }
}