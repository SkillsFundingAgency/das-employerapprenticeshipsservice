using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class UserInvitationsViewModel
    {
        public List<InvitationView> Invitations { get; set; }
        public bool ShowBreadCrumbs { get; set; }
    }
}
