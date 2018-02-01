using System;
using SFA.DAS.EAS.Domain.Data.Repositories;
using StructureMap;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper
{
    class DbBuilderDependentRepositories
    {
        private readonly Lazy<IAccountRepository> _lazyAccountRepository;
        private readonly Lazy<DbDirectRepository> _lazyDbDirectRepository;
        private readonly Lazy<IUserRepository> _lazyUserRepository;

        public DbBuilderDependentRepositories(IContainer container)
        {
            _lazyAccountRepository = new Lazy<IAccountRepository>(container.GetInstance<IAccountRepository>);
            _lazyDbDirectRepository = new Lazy<DbDirectRepository>(container.GetInstance<DbDirectRepository>);
            _lazyUserRepository = new Lazy<IUserRepository>(container.GetInstance<IUserRepository>);
        }

        public IAccountRepository AccountRepository => _lazyAccountRepository.Value;
        public DbDirectRepository DbDirectRepository => _lazyDbDirectRepository.Value;
        public IUserRepository UserRepository => _lazyUserRepository.Value;
    }
}