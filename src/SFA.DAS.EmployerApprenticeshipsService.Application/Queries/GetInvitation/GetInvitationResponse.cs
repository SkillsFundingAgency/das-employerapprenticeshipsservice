using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Application.Queries.GetInvitation
{
    public class GetInvitationResponse
    {
        public InvitationView Invitation { get; set; }
    }
}