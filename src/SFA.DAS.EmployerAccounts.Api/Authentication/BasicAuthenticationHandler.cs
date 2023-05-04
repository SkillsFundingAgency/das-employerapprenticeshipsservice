using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerAccounts.Api.Authorization;

namespace SFA.DAS.EmployerAccounts.Api.Authentication;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, "username"),
            new Claim(ClaimTypes.Name, "username"),
            new Claim(ClaimTypes.Role, ApiRoles.ReadUserAccounts),
            new Claim(ClaimTypes.Role, ApiRoles.ReadAllAccountUsers),
            new Claim(ClaimTypes.Role, ApiRoles.ReadAllEmployerAccountBalances),
            new Claim(ClaimTypes.Role, ApiRoles.ReadAllEmployerAgreements)
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}