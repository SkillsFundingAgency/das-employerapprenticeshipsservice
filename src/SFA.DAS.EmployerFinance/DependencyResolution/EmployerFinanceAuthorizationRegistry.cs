using SFA.DAS.Authorization.Handlers;
using SFA.DAS.EmployerFinance.AuthorisationExtensions;
using StructureMap;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class EmployerFinanceAuthorizationRegistry : Registry
    {
        public EmployerFinanceAuthorizationRegistry()
        {
            For<IAuthorizationHandler>().Add<EmployerFeatureAuthorizationHandler>();
        }
    }
}
