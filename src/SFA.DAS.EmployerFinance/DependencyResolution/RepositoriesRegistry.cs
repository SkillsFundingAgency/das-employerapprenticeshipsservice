using SFA.DAS.EmployerFinance.Data;
using StructureMap;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class RepositoriesRegistry : Registry
    {
        public RepositoriesRegistry()
        {
            For<IUserRepository>().Use<UserRepository>();
        }
    }
}