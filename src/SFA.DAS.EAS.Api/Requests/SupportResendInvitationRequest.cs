namespace SFA.DAS.EAS.Account.Api.Requests;

public class SupportResendInvitationRequest
{
    public string Email { get; set; }
    public string HashedAccountId { get; set; }
    public string FirstName { get; set; }
}