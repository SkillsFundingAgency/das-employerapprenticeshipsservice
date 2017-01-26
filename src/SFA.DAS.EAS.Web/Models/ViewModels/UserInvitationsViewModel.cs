using System.Collections.Generic;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Web.Models.ViewModels
{
    public class UserInvitationsViewModel
    {
        public List<InvitationView> Invitations { get; set; }
        public bool ShowBreadCrumbs { get; set; }
    }
}
