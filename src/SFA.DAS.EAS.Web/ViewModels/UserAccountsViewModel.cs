using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class UserAccountsViewModel
    {
        public Accounts<Domain.Models.Account.Account> Accounts;
        public int Invitations;
        public FlashMessageViewModel FlashMessage;
        public string ErrorMessage;
    }
}