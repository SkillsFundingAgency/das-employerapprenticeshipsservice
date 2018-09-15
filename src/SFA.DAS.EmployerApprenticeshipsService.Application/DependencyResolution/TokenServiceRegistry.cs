using SFA.DAS.Configuration;
using SFA.DAS.TokenService.Api.Client;
using StructureMap;
using TokenServiceApiClientConfiguration = SFA.DAS.EAS.Domain.Configuration.TokenServiceApiClientConfiguration;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class TokenServiceRegistry : Registry
    {
        public TokenServiceRegistry()
        {
            For<ITokenServiceApiClientConfiguration>().Use(c => c.GetInstance<TokenServiceApiClientConfiguration>());
            For<TokenServiceApiClientConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<TokenServiceApiClientConfiguration>("SFA.DAS.TokenServiceApiClient")).Singleton();
        }
    }
}