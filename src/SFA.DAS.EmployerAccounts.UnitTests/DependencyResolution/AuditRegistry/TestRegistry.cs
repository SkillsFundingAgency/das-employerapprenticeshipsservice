using SFA.DAS.AutoConfiguration;
using SFA.DAS.Authentication;
using StructureMap;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.UnitTests.DependencyResolution.AuditRegistry
{
    public class TestRegistry : Registry
    {
        public TestRegistry()
        {
            For<IEnvironmentService>().Use<TestEnvironmentService>();
            For<IAuthenticationService>().Use<TestAuthenticationService>();
        }

        public class TestEnvironmentService : IEnvironmentService
        {
            public string GetVariable(string variableName)
            {
                throw new System.NotImplementedException();
            }

            public bool IsCurrent(params DasEnv[] environment)
            {
                return true;
            }
        }

        public class TestAuthenticationService : IAuthenticationService
        {
            public string GetClaimValue(string key)
            {
                throw new System.NotImplementedException();
            }

            public bool HasClaim(string type, string value)
            {
                throw new System.NotImplementedException();
            }

            public bool IsUserAuthenticated()
            {
                throw new System.NotImplementedException();
            }

            public void SignOutUser()
            {
                throw new System.NotImplementedException();
            }

            public bool TryGetClaimValue(string key, out string value)
            {
                throw new System.NotImplementedException();
            }

            public Task UpdateClaims()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
