using SFA.DAS.EAS.Infrastructure.Authorization;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAuthorization
{
    public class GetTransferConnectionInvitationAuthorizationResponse
    {
        public AuthorizationResult AuthorizationResult { get; set; }
        public bool IsValidSender { get; set; }
    }
}