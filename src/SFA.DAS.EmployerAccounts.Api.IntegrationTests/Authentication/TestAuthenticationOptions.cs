using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.Authentication;

public class TestAuthenticationOptions : AuthenticationSchemeOptions
{
    public bool RequireBearerToken { get; set; }
    // customize as needed
    public virtual ClaimsIdentity Identity { get; set; } = new(
        new Claim[]
        {
            new("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", Guid.NewGuid().ToString()),
            new("http://schemas.microsoft.com/identity/claims/tenantid", "test"),
            new("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.NewGuid().ToString()),
            new("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", "test"),
            new("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname", "test"),
            new("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", "test"),
        },
        "test");

    public TestAuthenticationOptions() { }
}