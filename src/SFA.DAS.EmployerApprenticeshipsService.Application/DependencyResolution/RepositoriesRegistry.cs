using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Infrastructure.Data;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class RepositoriesRegistry : Registry
    {
        public RepositoriesRegistry()
        {
            For<IUserRepository>().Use<UserRepository>();
            For<IAccountRepository>().Use<AccountRepository>();
        }
    }
}