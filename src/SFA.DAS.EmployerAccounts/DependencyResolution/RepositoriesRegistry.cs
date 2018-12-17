using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Interfaces;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class RepositoriesRegistry : Registry
    {
        public RepositoriesRegistry()
        {
            For<IUserAccountRepository>().Use<UserAccountRepository>();
            For<IUserRepository>().Use<UserRepository>();
            For<IAccountRepository>().Use<AccountRepository>();
        }
    }
}