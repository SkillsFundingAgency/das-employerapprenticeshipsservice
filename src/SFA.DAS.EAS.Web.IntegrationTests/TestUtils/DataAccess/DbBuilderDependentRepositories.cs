using System;
using SFA.DAS.EAS.Domain.Data.Repositories;
using StructureMap;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess
{
    internal class DbBuilderDependentRepositories
    {
        private readonly Lazy<IAccountRepository> _lazyAccountRepository;
        private readonly Lazy<IUserRepository> _lazyUserRepository;
        private readonly Lazy<IDasLevyRepository> _lazyLevyRepository;

        public DbBuilderDependentRepositories(IContainer container)
        {
            _lazyAccountRepository = new Lazy<IAccountRepository>(container.GetInstance<IAccountRepository>);
            _lazyUserRepository = new Lazy<IUserRepository>(container.GetInstance<IUserRepository>);
            _lazyLevyRepository = new Lazy<IDasLevyRepository>(container.GetInstance<IDasLevyRepository>);
        }

        public IAccountRepository AccountRepository => _lazyAccountRepository.Value;
        public IUserRepository UserRepository => _lazyUserRepository.Value;
        public IDasLevyRepository DasLevyRepository => _lazyLevyRepository.Value;
    }
}