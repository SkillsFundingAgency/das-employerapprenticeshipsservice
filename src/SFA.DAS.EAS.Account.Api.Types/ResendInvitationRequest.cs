namespace SFA.DAS.EAS.Account.Api.Types;

public class ResendInvitationRequest
{
    public string HashedAccountId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string ExternalUserId { get; set; }
}