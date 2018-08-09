using SFA.DAS.EmployerAccounts.Models;
using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class UserInvitationsViewModel
    {
        public List<InvitationView> Invitations { get; set; }
        public bool ShowBreadCrumbs { get; set; }
    }
}
