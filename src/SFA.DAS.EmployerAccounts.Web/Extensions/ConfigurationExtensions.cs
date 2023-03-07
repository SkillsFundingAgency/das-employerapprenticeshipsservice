namespace SFA.DAS.EmployerAccounts.Web.Extensions;

public static class ConfigurationExtensions
{
    public static bool UseGovUkSignIn(this IConfiguration configuration)
    {
        return configuration["EmployerAccountsConfiguration:UseGovSignIn"] != null &&
               configuration["EmployerAccountsConfiguration:UseGovSignIn"]
                  .Equals("true", StringComparison.CurrentCultureIgnoreCase);
    }

    public static bool UseStubAuth(this IConfiguration configuration)
    {
        return configuration["StubAuth"] != null && configuration["StubAuth"]
            .Equals("true", StringComparison.CurrentCultureIgnoreCase);
    }
}