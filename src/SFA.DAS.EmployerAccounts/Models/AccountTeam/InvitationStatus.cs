namespace SFA.DAS.EmployerAccounts.Models.AccountTeam;

public enum InvitationStatus : byte
{
    Pending = 1,
    Accepted = 2,
    Expired = 3,
    Deleted = 4
}

public static class InvitationStatusStrings
{
    public static string ToString(InvitationStatus status)
    {
        switch (status)
        {
            case InvitationStatus.Pending: return "Invitation awaiting response";
            case InvitationStatus.Accepted: return "Active";
            case InvitationStatus.Deleted: return "Invitation cancelled";
            case InvitationStatus.Expired: return "Invitation expired";
            default: throw new ArgumentException("unexpected InvitationStatus: " + status);
        }
    }
}