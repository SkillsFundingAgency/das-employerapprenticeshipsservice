using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.TokenService.Api.Client;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class TokenServiceRegistry : Registry
    {
        public TokenServiceRegistry()
        {
            For<ITokenServiceApiClientConfiguration>().Use(c => c.GetInstance<TokenServiceApiClientConfiguration>());
            For<TokenServiceApiClientConfiguration>().Use(c => c.GetInstance<EmployerApprenticeshipsServiceConfiguration>().TokenServiceApi).Singleton();
        }
    }
}