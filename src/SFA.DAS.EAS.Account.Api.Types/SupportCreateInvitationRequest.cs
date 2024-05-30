namespace SFA.DAS.EAS.Account.Api.Types;

public class SupportCreateInvitationRequest
{
    public string HashedAccountId { get; set; }

    public string NameOfPersonBeingInvited { get; set; }

    public string EmailOfPersonBeingInvited { get; set; }

    public int RoleOfPersonBeingInvited { get; set; }
    public string SupportUserEmail { get; set; }
}