using System.Security.Claims;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.Infrastructure;

namespace SFA.DAS.EmployerAccounts.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetDisplayName(this ClaimsPrincipal user)
    {
        return user.FindFirst(EmployerClaims.IdamsUserIdClaimTypeIdentifier)?.Value;
    }

    public static string GetEmailAddress(this ClaimsPrincipal user)
    {
        return user.FindFirst(EmployerClaims.IdamsUserIdClaimTypeIdentifier)?.Value;
    }

    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirst(EmployerClaims.IdamsUserIdClaimTypeIdentifier)?.Value;
    }

    public static IEnumerable<string> GetEmployerAccounts(this ClaimsPrincipal user)
    {
        var employerAccountClaim = user.FindFirst(c =>
            c.Type.Equals(EmployerClaims.AccountsClaimsTypeIdentifier));

        if (string.IsNullOrEmpty(employerAccountClaim?.Value))
            return Enumerable.Empty<string>();

        return JsonConvert.DeserializeObject<List<string>>(employerAccountClaim.Value);
    }
}