using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserInvitations
{
    public class GetUserInvitationsResponse
    {
        public List<InvitationView> Invitations { get; set; }
    }
}