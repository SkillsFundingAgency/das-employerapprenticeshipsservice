using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUserInvitations
{
    public class GetUserInvitationsResponse
    {
        public List<InvitationView> Invitations { get; set; }
    }
}