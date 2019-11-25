namespace SFA.DAS.EmployerAccounts.Web.Authorization
{
    public static class AuthorizationConstants
    {
        public const string Tier2User = "Tier2User";
        public const string TeamViewRoute = "accounts/{hashedaccountid}/teams/view";
        public const string TeamRoute = "accounts/{hashedaccountid}/teams";
        public const string TeamInvite = "accounts/{hashedaccountid}/teams/invite";
        public const string TeamReview = "accounts/{hashedaccountid}/teams/{email}/review/";
    }
}