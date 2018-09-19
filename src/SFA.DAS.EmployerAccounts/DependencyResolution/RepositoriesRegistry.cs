using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Data;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class RepositoriesRegistry : Registry
    {
        public RepositoriesRegistry()
        {
            For<IUserAccountRepository>().Use<UserAccountRepository>();
        }
    }
}