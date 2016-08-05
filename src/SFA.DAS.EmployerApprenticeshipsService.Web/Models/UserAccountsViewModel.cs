using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class UserAccountsViewModel
    {
        public Accounts Accounts;
        public int Invitations;
    }
}