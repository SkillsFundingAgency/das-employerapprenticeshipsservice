﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.EAS.Application.Infrastructure;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Web.Services;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.EAS.Web.Handlers;

public class EmployerAccountPostAuthenticationClaimsHandler : ICustomClaims
{
    private readonly IUserAccountService _userAccountService;
    private readonly EmployerApprenticeshipsServiceConfiguration _employerAccountsConfiguration;

    public EmployerAccountPostAuthenticationClaimsHandler(IUserAccountService userAccountService, IOptions<EmployerApprenticeshipsServiceConfiguration> employerAccountsConfiguration)
    {
        _userAccountService = userAccountService;
        _employerAccountsConfiguration = employerAccountsConfiguration.Value;
    }

    public async Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext tokenValidatedContext)
    {
        var claims = new List<Claim>();

        string userId;
        var email = string.Empty;

        if (_employerAccountsConfiguration.UseGovSignIn)
        {
            userId = tokenValidatedContext.Principal.Claims
                .First(c => c.Type.Equals(ClaimTypes.NameIdentifier))
                .Value;
            email = tokenValidatedContext.Principal.Claims
                .First(c => c.Type.Equals(ClaimTypes.Email))
                .Value;
            claims.Add(new Claim(EmployerClaims.IdamsUserEmailClaimTypeIdentifier, email));
        }
        else
        {
            userId = tokenValidatedContext.Principal.Claims
                .First(c => c.Type.Equals(EmployerClaims.IdamsUserIdClaimTypeIdentifier))
                .Value;

            email = tokenValidatedContext.Principal.Claims
                .First(c => c.Type.Equals(EmployerClaims.IdamsUserEmailClaimTypeIdentifier)).Value;

            claims.AddRange(tokenValidatedContext.Principal.Claims);
            claims.Add(new Claim(EmployerClaims.IdamsUserIdClaimTypeIdentifier, userId));
        }

        var result = await _userAccountService.GetUserAccounts(userId, email);

        var accountsAsJson = JsonConvert.SerializeObject(result.EmployerAccounts.ToDictionary(k => k.AccountId));
        var associatedAccountsClaim = new Claim(EmployerClaims.AccountsClaimsTypeIdentifier, accountsAsJson, JsonClaimValueTypes.Json);
        claims.Add(associatedAccountsClaim);

        if (!_employerAccountsConfiguration.UseGovSignIn)
        {
            return claims;
        }

        if (result.IsSuspended)
        {
            claims.Add(new Claim(ClaimTypes.AuthorizationDecision, "Suspended"));
        }

        claims.Add(new Claim(EmployerClaims.IdamsUserIdClaimTypeIdentifier, result.EmployerUserId));
        claims.Add(new Claim(EmployerClaims.IdamsUserDisplayNameClaimTypeIdentifier, result.FirstName + " " + result.LastName));
        claims.Add(new Claim(DasClaimTypes.GivenName, result.FirstName));
        claims.Add(new Claim(DasClaimTypes.FamilyName, result.LastName));

        return claims;
    }
}