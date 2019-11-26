namespace SFA.DAS.EmployerAccounts.Web.Authorization
{
    public static class AuthorizationConstants
    {
        public const string Tier2User = "Tier2User";
        public const string TeamViewRoute = "accounts/{hashedaccountid}/teams/view";
        public const string TeamRoute = "accounts/{hashedaccountid}/teams";
        public const string TeamInvite = "accounts/{hashedaccountid}/teams/invite";
        public const string TeamInviteComplete = "accounts/{hashedaccountid}/teams/invite/next";
        public const string TeamMemberRemove = "accounts/{hashedaccountid}/teams/remove";
        public const string TeamReview = "accounts/{hashedaccountid}/teams/{email}/review";
        public const string TeamMemberRoleChange = "accounts/{hashedaccountid}/teams/{email}/role/change";
        public const string TeamMemberInviteResend = "accounts/{hashedaccountid}/teams/resend";
        public const string TeamMemberInviteCancel = "accounts/{hashedaccountid}/teams/{invitationId}/cancel";
    }
}