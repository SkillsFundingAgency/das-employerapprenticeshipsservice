using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.EmployerUsers;
using SFA.DAS.EmployerAccounts.Infrastructure;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.EmployerAccounts.Web;

public class EmployerAccountPostAuthenticationClaimsHandler : ICustomClaims
{
    private readonly IEmployerAccountService _accountsSvc;
    private readonly IConfiguration _configuration;
    private readonly EmployerAccountsConfiguration _employerAccountsConfiguration;

    public EmployerAccountPostAuthenticationClaimsHandler(IEmployerAccountService accountsSvc, IConfiguration configuration, IOptions<EmployerAccountsConfiguration> forecastingConfiguration)
    {
        _accountsSvc = accountsSvc;
        _configuration = configuration;
        _employerAccountsConfiguration = forecastingConfiguration.Value;
    }
    public async Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext tokenValidatedContext)
    {
        var claims = new List<Claim>();
        if (_configuration["StubAuth"] != null && _configuration["StubAuth"]
                .Equals("true", StringComparison.CurrentCultureIgnoreCase))
        {
            var accountClaims = new Dictionary<string, EmployerUserAccountItem>();
            accountClaims.Add("", new EmployerUserAccountItem
            {
                Role = "Owner",
                AccountId = "ABC123",
                EmployerName = "Stub Employer"
            });
            claims.AddRange(new[]
            {
                new Claim(EmployerClaims.AccountsClaimsTypeIdentifier, JsonConvert.SerializeObject(accountClaims)),
                new Claim(EmployerClaims.IdamsUserEmailClaimTypeIdentifier, _configuration["NoAuthEmail"]),
                new Claim(EmployerClaims.IdamsUserIdClaimTypeIdentifier, Guid.NewGuid().ToString())
            });
            return claims.ToList();
        }

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
        }

        var result = await _accountsSvc.GetUserAccounts(userId, email);

        var accountsAsJson = JsonConvert.SerializeObject(result.EmployerAccounts.ToDictionary(k => k.AccountId));
        var associatedAccountsClaim = new Claim(EmployerClaims.AccountsClaimsTypeIdentifier, accountsAsJson, JsonClaimValueTypes.Json);
        claims.Add(associatedAccountsClaim);
        if (!_employerAccountsConfiguration.UseGovSignIn)
        {
            return claims;
        }

        claims.Add(new Claim(EmployerClaims.IdamsUserIdClaimTypeIdentifier, result.EmployerUserId));
        claims.Add(new Claim(EmployerClaims.IdamsUserDisplayNameClaimTypeIdentifier, result.FirstName + " " + result.LastName));

        return claims;
    }
}