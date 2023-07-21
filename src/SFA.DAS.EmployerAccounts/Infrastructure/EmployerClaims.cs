namespace SFA.DAS.EmployerAccounts.Infrastructure;

public static class EmployerClaims
{
    public static string IdamsUserIdClaimTypeIdentifier => "http://das/employer/identity/claims/id";
    public static string AccountsClaimsTypeIdentifier => "http://das/employer/identity/claims/associatedAccounts";
    public static string IdamsUserDisplayNameClaimTypeIdentifier => "http://das/employer/identity/claims/display_name";
    public static string IdamsUserEmailClaimTypeIdentifier => "http://das/employer/identity/claims/email_address";
}