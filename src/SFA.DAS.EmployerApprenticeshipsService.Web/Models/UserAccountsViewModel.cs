using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class UserAccountsViewModel
    {
        public Accounts Accounts;
        public int Invitations;
        public FlashMessageViewModel FlashMessage;
        public string ErrorMessage;
    }
}