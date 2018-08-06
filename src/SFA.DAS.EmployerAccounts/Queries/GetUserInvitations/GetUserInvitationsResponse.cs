using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using System.Collections.Generic;

namespace SFA.DAS.Queries.GetUserInvitations
{
    public class GetUserInvitationsResponse
    {
        public List<InvitationView> Invitations { get; set; }
    }
}