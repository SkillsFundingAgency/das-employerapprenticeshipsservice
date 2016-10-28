using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Web.Models
{
    public class UserAccountsViewModel
    {
        public Accounts Accounts;
        public int Invitations;
        public FlashMessageViewModel FlashMessage;
        public string ErrorMessage;
    }
}