namespace SFA.DAS.EAS.Account.Api.Types;

public record ResendInvitationRequest
{
    public ResendInvitationRequest(string HashedAccountId, string Email, string FirstName, string ExternalUserId) { }
}