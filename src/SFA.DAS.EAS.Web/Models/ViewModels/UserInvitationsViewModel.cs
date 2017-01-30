using System.Collections.Generic;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Web.Models.ViewModels
{
    public class UserInvitationsViewModel
    {
        public List<InvitationView> Invitations { get; set; }
        public bool ShowBreadCrumbs { get; set; }
    }
}
