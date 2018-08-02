using SFA.DAS.EmployerAccounts.Data;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class RepositoriesRegistry : Registry
    {
        public RepositoriesRegistry()
        {
            For<IUserRepository>().Use<UserRepository>();
        }
    }
}