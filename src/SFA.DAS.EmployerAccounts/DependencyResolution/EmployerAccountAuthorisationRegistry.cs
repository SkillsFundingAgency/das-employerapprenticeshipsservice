using SFA.DAS.Authorization.Handlers;
using SFA.DAS.EmployerAccounts.AuthorisationExtensions;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class EmployerAccountAuthorisationExtensionRegistry : Registry
    {
        public EmployerAccountAuthorisationExtensionRegistry()
        {
            For<IAuthorizationHandler>().Add<EmployerFeatureAuthorizationHandler>();
        }
    }
}
