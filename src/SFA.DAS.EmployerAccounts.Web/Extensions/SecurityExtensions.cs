﻿using System.Security.Claims;
using System.Security.Principal;
using SFA.DAS.EmployerAccounts.Web.Infrastructure;

namespace SFA.DAS.EmployerAccounts.Web.Extensions;

public static class SecurityExtensions
{
    public static string HashedAccountId(this IIdentity identity)
    {
        var claimsIdentity = identity as ClaimsIdentity;
        var claim = claimsIdentity?.Claims.FirstOrDefault(c => c.Type == RouteValues.EncodedAccountId);
        return (!string.IsNullOrEmpty(claim?.Value)) ? claim?.Value : string.Empty;
    }
}