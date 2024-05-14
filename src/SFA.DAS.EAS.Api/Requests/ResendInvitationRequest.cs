namespace SFA.DAS.EAS.Account.Api.Requests;

public class ResendInvitationRequest
{
    public string Email { get; set; }
    public string HashedAccountId { get; set; }
    public string ExternalUserId { get; set; }
    public string FirstName { get; set; }
}