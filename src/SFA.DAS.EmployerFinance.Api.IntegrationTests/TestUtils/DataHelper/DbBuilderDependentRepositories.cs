using System;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Interfaces;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataHelper
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