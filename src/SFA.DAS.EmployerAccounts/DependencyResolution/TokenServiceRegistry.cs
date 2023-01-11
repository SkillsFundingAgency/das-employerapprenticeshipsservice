using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.TokenService.Api.Client;

namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class TokenServiceRegistry : Registry
{
    public TokenServiceRegistry()
    {
        For<ITokenServiceApiClientConfiguration>().Use(c => c.GetInstance<TokenServiceApiClientConfiguration>());
        For<TokenServiceApiClientConfiguration>().Use(c => c.GetInstance<EmployerAccountsConfiguration>().TokenServiceApi).Singleton();
    }
}