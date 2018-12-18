using System;
using SFA.DAS.EmployerFinance.Data;
using StructureMap;

namespace SFA.DAS.EmployerFinance.Api.IntegrationTests.TestUtils.DataAccess
{
    class DbBuilderDependentRepositories
    {
        private readonly Lazy<IAccountRepository> _lazyAccountRepository;
        private readonly Lazy<IUserRepository> _lazyUserRepository;

        public DbBuilderDependentRepositories(IContainer container)
        {
            _lazyAccountRepository = new Lazy<IAccountRepository>(container.GetInstance<IAccountRepository>);
            _lazyUserRepository = new Lazy<IUserRepository>(container.GetInstance<IUserRepository>);
        }

        public IAccountRepository AccountRepository => _lazyAccountRepository.Value;
        public IUserRepository UserRepository => _lazyUserRepository.Value;
    }
}