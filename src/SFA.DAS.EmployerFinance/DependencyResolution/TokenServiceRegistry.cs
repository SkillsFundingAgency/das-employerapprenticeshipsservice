using SFA.DAS.AutoConfiguration;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.TokenService.Api.Client;
using StructureMap;
using TokenServiceApiClientConfiguration = SFA.DAS.EmployerFinance.Configuration.TokenServiceApiClientConfiguration;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class TokenServiceRegistry : Registry
    {
        public TokenServiceRegistry()
        {
            For<ITokenServiceApiClientConfiguration>().Use(c => c.GetInstance<TokenServiceApiClientConfiguration>());
            For<TokenServiceApiClientConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<TokenServiceApiClientConfiguration>(ConfigurationKeys.TokenServiceApiClient)).Singleton();
        }
    }
}