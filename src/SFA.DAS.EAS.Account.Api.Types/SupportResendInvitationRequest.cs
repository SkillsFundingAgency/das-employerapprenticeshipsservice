namespace SFA.DAS.EAS.Account.Api.Types;

public class SupportResendInvitationRequest
{
    public string HashedAccountId { get; set; }
    public string Email { get; set; }
}