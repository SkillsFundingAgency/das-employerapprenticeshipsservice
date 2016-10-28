using System.Collections.Generic;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Application.Queries.GetUserInvitations
{
    public class GetUserInvitationsResponse
    {
        public List<InvitationView> Invitations { get; set; }
    }
}