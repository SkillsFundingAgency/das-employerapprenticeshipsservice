using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferConnectionInvitationAuthorization
{
    public class GetTransferConnectionInvitationAuthorizationResponse
    {
        public AuthorizationResult AuthorizationResult { get; set; }
        public bool IsValidSender { get; set; }
    }
}