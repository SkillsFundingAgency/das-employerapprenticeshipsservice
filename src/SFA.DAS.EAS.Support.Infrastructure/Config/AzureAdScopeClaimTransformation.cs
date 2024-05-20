using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;

namespace SFA.DAS.EAS.Support.Infrastructure.Config;

public class AzureAdScopeClaimTransformation : IClaimsTransformation
{
    private readonly ILogger<AzureAdScopeClaimTransformation> _logger;

    public AzureAdScopeClaimTransformation(ILogger<AzureAdScopeClaimTransformation> logger)
    {
        _logger = logger;
    }
    
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        _logger.LogInformation("AzureAdScopeClaimTransformation.TransformAsync claims {Claims}", JsonConvert.SerializeObject(principal.Claims.Select(x => new
        {
            x.Type,
            x.Value
        })));
        
        return Task.FromResult(principal);
        
        var scopeClaims = principal.FindAll(Constants.ScopeClaimType).ToList();
        if (scopeClaims.Count != 1 || !scopeClaims[0].Value.Contains(' '))
        {
            // Caller has no scopes or has multiple scopes (already split)
            // or they have only one scope
            return Task.FromResult(principal);
        }

        var claim = scopeClaims[0];
        var scopes = claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var claims = scopes.Select(s => new Claim(Constants.ScopeClaimType, s));

        return Task.FromResult(new ClaimsPrincipal(new ClaimsIdentity(principal.Identity, claims)));
    }
}