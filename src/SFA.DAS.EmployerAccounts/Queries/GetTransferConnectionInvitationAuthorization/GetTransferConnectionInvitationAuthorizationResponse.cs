using SFA.DAS.Authorization;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferConnectionInvitationAuthorization
{
    public class GetTransferConnectionInvitationAuthorizationResponse
    {
        public AuthorizationResult AuthorizationResult { get; set; }
        public bool IsValidSender { get; set; }
        public decimal TransferAllowancePercentage { get; set; }
    }
}