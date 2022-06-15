using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.TokenService.Api.Client;
using StructureMap;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class TokenServiceRegistry : Registry
    {
        public TokenServiceRegistry()
        {
            For<ITokenServiceApiClientConfiguration>().Use(c => c.GetInstance<TokenServiceApiClientConfiguration>());
            For<TokenServiceApiClientConfiguration>().Use(c => c.GetInstance<EmployerFinanceConfiguration>().TokenServiceApi).Singleton();
        }
    }
}