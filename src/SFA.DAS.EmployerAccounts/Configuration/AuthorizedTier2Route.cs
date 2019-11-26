namespace SFA.DAS.EmployerAccounts.Configuration
{
    public static class AuthorizedTier2Route
    {
        public const string TeamViewRoute = "accounts/{hashedaccountid}/teams/view";
        public const string TeamInvite = "accounts/{hashedaccountid}/teams/invite";
        public const string TeamInviteComplete = "accounts/{hashedaccountid}/teams/invite/next";
        public const string TeamMemberRemove = "accounts/{hashedaccountid}/teams/{email}/remove";
        public const string TeamReview = "accounts/{hashedaccountid}/teams/{email}/review";
        public const string TeamMemberRoleChange = "accounts/{hashedaccountid}/teams/{email}/role/change";
        public const string TeamMemberInviteResend = "accounts/{hashedaccountid}/teams/resend";
        public const string TeamMemberInviteCancel = "accounts/{hashedaccountid}/teams/{invitationId}/cancel";
        public const string TeamRoute = "accounts/{hashedaccountid}/teams";
    }
}
