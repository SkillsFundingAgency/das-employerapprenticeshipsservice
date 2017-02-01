using System.Collections.Generic;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Application.Queries.GetUserInvitations
{
    public class GetUserInvitationsResponse
    {
        public List<InvitationView> Invitations { get; set; }
    }
}