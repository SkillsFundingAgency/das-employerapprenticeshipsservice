using Microsoft.AspNetCore.Authentication;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.Authentication;

public static class TestAuthenticationExtensions
{
    public static AuthenticationBuilder AddTestAuthentication(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<TestAuthenticationOptions> configureOptions)
    {
        return builder.AddScheme<TestAuthenticationOptions, TestAuthHandler>(authenticationScheme, configureOptions);
    }
}